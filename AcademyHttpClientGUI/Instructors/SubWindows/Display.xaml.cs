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
        }

        private static async Task<List<Instructor>> GetInstructors()
        {
            List<Instructor> instructors = new();
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync("https://localhost:44331/api/instructor");
            response.EnsureSuccessStatusCode();
            instructors = await response.Content.ReadAsAsync<List<Instructor>>();

            if(instructors.Any()) return instructors;
            return null;
        }

        private async void Onload()
        {

        }
    }
}
