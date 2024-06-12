using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using static System.Net.WebRequestMethods;

namespace LocationApp
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient client = new HttpClient();
        private const string apiKey = "7984a54699694596a261db4e805007fb";

        public Form1()
        {
            InitializeComponent();
            InitializeDataGridView();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string query = textBox1.Text;
            string apiUrl = $"https://api.geoapify.com/v1/geocode/search?text={query}&apiKey={apiKey}";

            progressBar1.Visible = true;

            try
            {
                string response = await GetApiResponse(apiUrl);
                JObject jsonResponse = JObject.Parse(response);

                var locations = new List<Location>();
                foreach (var feature in jsonResponse["features"])
                {
                    var properties = feature["properties"];
                    var location = new Location
                    {
                        Formatted = properties["formatted"].ToString(),
                        Lat = (double)properties["lat"],
                        Lon = (double)properties["lon"],
                        Country = properties["country"].ToString(),
                        State = properties["state"].ToString(),
                        City = properties["city"].ToString()
                    };
                    locations.Add(location);
                }
                MessageBox.Show($"Number of locations searched: {locations.Count}");
                dataGridView1.DataSource = locations;

                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.Message);
            }
            finally
            {
                progressBar1.Visible = false;
            }

        }
        private async Task<string> GetApiResponse(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public class Location
        {
            public string Formatted { get; set; }
            public double Lat { get; set; }
            public double Lon { get; set; }
            public string Country { get; set; }
            public string State { get; set; }
            public string City { get; set; }
        }

        private void InitializeDataGridView()
        {
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Formatted",
                HeaderText = "Address"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Lat",
                HeaderText = "Latitude"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Lon",
                HeaderText = "Longitude"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Country",
                HeaderText = "Country"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "State",
                HeaderText = "State"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "City",
                HeaderText = "City"
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            progressBar1.Visible = false;
        }

       
    }
}
