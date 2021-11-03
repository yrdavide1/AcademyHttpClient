using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
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

namespace AcademyHttpClientGUI.Instructors.SubWindows
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

        private static async Task<List<Instructor>> GetInstructors()
        {
            List<Instructor> instructors = new();
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync("https://localhost:44331/api/instructor");
            response.EnsureSuccessStatusCode();
            instructors = await response.Content.ReadAsAsync<List<Instructor>>();

            return instructors;
        }

        private async void Onload()
        {
            try
            {
                double height = 300;
                List<Instructor> instructors = await GetInstructors();
                int elements = instructors.Count;
                DisplayBlock.Text = "";

                foreach (Instructor i in instructors)
                {
                    List<PropertyInfo> props = i.GetType().GetProperties().ToList();

                    foreach (PropertyInfo prop in props)
                    {
                        if (prop == null) DisplayBlock.Text += $"{prop.Name}: PROPERTY IS NULL\n";
                        else DisplayBlock.Text += $"{prop.Name}: {prop.GetValue(i)}\n";
                    }
                    DisplayBlock.Text += "-------------";
                    height *= elements;
                    this.Height = height;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Cannot retrieve instructors from DB!", ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
                this.Loaded += (sender, e) => Close();
            }
        }
    }
}
