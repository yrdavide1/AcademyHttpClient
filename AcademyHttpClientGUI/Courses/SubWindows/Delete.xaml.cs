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
    /// Logica di interazione per Delete.xaml
    /// </summary>
    public partial class Delete : Window
    {
        public Delete()
        {
            InitializeComponent();
            try
            {
                Onload();
            }
            catch (Exception ex)
            {
                Close();
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ClickDelete(object sender, RoutedEventArgs e)
        {
            Dictionary<string, bool> result = await DeleteCourse("CoursesList");
            if (result.First().Value)
            {
                MessageBox.Show($"Course with title {result.First().Key} successfully deleted");
                Close();
            }
        }

        private static async Task<Dictionary<long, string>> GetCourses()
        {
            List<Course> courses = new();
            Dictionary<long, string> coursesMap = new(); 

            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync("https://localhost:44331/api/course/");
            response.EnsureSuccessStatusCode();
            courses = await response.Content.ReadAsAsync<List<Course>>();
            courses.ForEach(e =>
            {
                coursesMap.Add(e.Id, e.Title);
            });

            return coursesMap;
        }

        private async void Onload()
        {
            Dictionary<long, string> courses = await GetCourses();
            TextBlock info = new()
            {
                Text = "Select course to delete\nCourses are listed by title",
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 15,
                Margin = new Thickness(0, 20, 0, 0)
            };
            ComboBox coursesList = new()
            {
                Width = 200,
                Margin = new Thickness(0, 20, 0, 0),
                Name = "CoursesList"
            };
            Button confirm = new()
            {
                Content = "Confirm",
                Height = 40,
                Width = 60,
                Margin = new Thickness(280, 80, 0, 0)
            };

            coursesList.Items.Insert(0, "COURSES");
            coursesList.SelectedIndex = 0;
            confirm.Click += new RoutedEventHandler(ClickDelete);
            Container.Children.Add(info);
            Container.Children.Add(coursesList);
            Container.Children.Add(confirm);

            confirm.Loaded += (sender, e) =>
            {
                Border border = confirm.Template.FindName("border", confirm) as Border;
                if (border != null)
                {
                    border.CornerRadius = new CornerRadius(5);
                }
            };

            coursesList.Loaded += (sender, e) =>
            {
                foreach((long key, string value) in courses)
                {
                    coursesList.Items.Add(value);
                }
            };
        }

        private async Task<Dictionary<string, bool>> DeleteCourse(string comboBoxName)
        {
            Dictionary<long, string> courses = await GetCourses();
            Dictionary<string, bool> result = new();
            IEnumerable<ComboBox> collection = Container.Children.OfType<ComboBox>();
            string selectedTitle = collection.First().SelectedItem as string;
            long idToDelete = 0;
            
            foreach(KeyValuePair<long, string> pair in courses)
            {
                if(pair.Value == selectedTitle) idToDelete = pair.Key;
            }

            using HttpClient client = new();
            HttpResponseMessage response = await client.DeleteAsync($"https://localhost:44331/api/course/{idToDelete}");
            response.EnsureSuccessStatusCode();
            result.Add(selectedTitle, response.IsSuccessStatusCode);

            return result;
        }
    }
}
