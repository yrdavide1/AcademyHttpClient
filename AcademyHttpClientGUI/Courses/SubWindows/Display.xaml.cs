using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AcademyHttpClientGUI.Courses;

namespace AcademyHttpClientGUI.Courses.SubWindows
{
    /// <summary>
    /// Logica di interazione per Display.xaml
    /// </summary>
    public partial class Display : Window
    {
        public Display()
        {
            InitializeComponent();
            Onload();
        }

        private async void Onload()
        {
            DisplayText.Text = "";
            try
            {
                List<Course> courses = new();
                using HttpClient client = new();
                HttpResponseMessage response = await client.GetAsync($"https://localhost:44331/api/course");
                response.EnsureSuccessStatusCode();

                courses = await response.Content.ReadAsAsync<List<Course>>();
                if (courses.Any())
                {
                    foreach(Course c in courses)
                    {
                        foreach(var p in c.GetType().GetProperties())
                        {
                            if (p.Name != "AreaName")
                            {
                                if (p.Name == "BasePrice") DisplayText.Text += $"{p.Name}: {p.GetValue(c)}$\n";
                                else DisplayText.Text += $"{p.Name}: {p.GetValue(c)}\n";
                            }
                        }
                        DisplayText.Text += "-------------\n";
                    }
                }
                else
                {
                    DisplayText.Text = $"Server error code {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Close();
                MessageBox.Show(ex.Message, ex.Source);
            }
        }
    }
}
