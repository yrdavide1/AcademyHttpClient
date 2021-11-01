using System;
using System.Collections;
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
    /// Logica di interazione per Modify.xaml
    /// </summary>
    public partial class Modify : Window
    {
        public Modify()
        {
            InitializeComponent();
            Onload();
        }

        private async void Onload()
        {
            List<Course> courses = await GetCourses();
            Dictionary<string, FrameworkElement> controls = new();
            Dictionary<long, string> coursesMap = new();

            foreach(Course c in courses)
            {
                coursesMap.Add(c.Id, c.Title);
            }

            TextBlock info = new()
            {
                Text = "Select a course, then select a property to modify",
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 15,
                Margin = new Thickness(0, 20, 0, 0)
            };
            TextBlock courseInfo = new()
            {
                Text="Courses list",
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 15,
                Margin = new Thickness(20, 20, 0, 0),
            };
            ComboBox coursesList = new()
            {
                Width = 200,
                Margin = new Thickness(20, 20, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                Name = "CoursesList"
            };
            TextBlock propsInfo = new()
            {
                Text = "Properties list",
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 15,
                Margin = new Thickness(20, 20, 0, 0),
                Name = "PropInfo",
                Visibility = Visibility.Hidden
            };
            ComboBox propsList = new()
            {
                Width = 200,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(20, 20, 0, 0),
                Name = "PropsList",
                Visibility = Visibility.Hidden
            };
            Button confirm = new()
            {
                Content = "Confirm",
                Height = 40,
                Width = 60,
                Margin = new Thickness(280, 40, 0, 0)
            };

            coursesList.Items.Insert(0, "COURSES");
            coursesList.SelectedIndex = 0;
            propsList.Items.Insert(0, "PROPERTIES");
            propsList.SelectedIndex = 0;
            controls.Add(propsList.Name, propsList);
            controls.Add(propsInfo.Name, propsInfo);

            Container.Children.Add(info);
            Container.Children.Add(courseInfo);
            Container.Children.Add(coursesList);
            Container.Children.Add(propsInfo);
            Container.Children.Add(propsList);
            Container.Children.Add(confirm);

            confirm.Loaded += (sender, e) =>
            {
                Border border = confirm.Template.FindName("border", confirm) as Border;

                if(border != null)
                {
                    border.CornerRadius = new CornerRadius(5);
                }
            };

            coursesList.Loaded += (sender, e) =>
            {
                foreach(KeyValuePair<long, string> pair in coursesMap)
                {
                    coursesList.Items.Add(pair.Value);
                }
            };

            confirm.Click += (sender, e) => 
            {
                //modify course prop
            };

            coursesList.SelectionChanged += (sender, e) =>
            {
                //pair.value.items.add(course props)
                foreach(var c in controls)
                {
                    c.Value.Visibility = Visibility.Visible;
                }

                Course selectedCourse = courses.Where(x => x.Title == coursesList.SelectedItem as string).First();
                foreach(var props in selectedCourse.GetType().GetProperties())
                {
                    if (props.Name != "Id") propsList.Items.Add(props.Name);
                }
            };
        }

        private static async Task<List<Course>> GetCourses()
        {
            List<Course> courses = new();

            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync("https://localhost:44331/api/course/");
            response.EnsureSuccessStatusCode();
            courses = await response.Content.ReadAsAsync<List<Course>>();

            return courses;
        }
    }
}
