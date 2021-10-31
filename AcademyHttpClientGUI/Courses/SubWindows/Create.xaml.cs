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

namespace AcademyHttpClientGUI.Courses.SubWindows
{
    /// <summary>
    /// Logica di interazione per Create.xaml
    /// </summary>
    /// 

    public partial class Create : Window
    {
        public Create()
        {
            InitializeComponent();
            Onload();
        }

        private async void Confirm(object sender, RoutedEventArgs e)
        {
            Course course = new();
            course.Id = 0;
            course.Title = Title.Text;
            course.Description = Description.Text;

            if (int.TryParse(Title.Text, out int duration)) course.Duration = duration;
            
            if(int.TryParse(Price.Text, out int price)) course.BasePrice = price;

            course.Syllabus = Syllabus.Text;
            course.Level = LevelList.SelectedIndex;
            course.AreaId = AreaIds.SelectedIndex + 1;

            if (GrantsCertification.IsChecked != null) course.GrantsCertification = (bool)GrantsCertification.IsChecked;

            course.CreationDate = FormatDate(CreationDate.Text); //dd-MM-yyyy

            MessageBox.Show((course.AreaId + 1).ToString());

            using HttpClient client = new();
            HttpResponseMessage response = await client.PostAsJsonAsync("https://localhost:44331/api/course/", course);
            response.EnsureSuccessStatusCode();

            response = await client.GetAsync($"https://localhost:44331/api/course/area/{course.AreaId}");
            response.EnsureSuccessStatusCode();
            //long createdId = (await response.Content.ReadAsAsync<List<Course>>()).First().Id;

            MessageBox.Show($"Course successfully created");
            Close();
        }

        private async void Onload()
        {
            List<Area> areas = new();
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync("https://localhost:44331/api/course/areas");
            response.EnsureSuccessStatusCode();

            areas = await response.Content.ReadAsAsync<List<Area>>();
            areas.ForEach(e => AreaIds.Items.Add(e.Id));

            Dictionary<long, string> levels = new Dictionary<long, string>
            {
                { 0, "BEGINNER" },
                { 1, "INTERMEDIATE"},
                { 2, "ADVANCED"},
                { 3, "GURU"}
            };

            foreach((long key, string value) in levels)
            {
                LevelList.Items.Add(value);
            }
        }

        private string FormatDate(string date)
        {
            string[] x = date.Split('/');
            string[] formattedDate =
            {
                x[2],
                x[1],
                x[0]
            };

            return string.Join('-', formattedDate);
        }
    }
}
