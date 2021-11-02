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
            Label inputLabel = new()
            {
                Visibility = Visibility.Hidden,
                Margin = new Thickness(17, 20, 0, 0),
            };
            TextBox input = new()
            {
                Width = 220,
                HorizontalAlignment= HorizontalAlignment.Left,
                Name = "PropInput",
                Margin = new Thickness(20, 0, 0, 0)
            };
            CheckBox grInput = new()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Content = "Grants certification?",
                Name = "GrantsCertification",
                Margin = new Thickness(20, 0, 0, 0)
            };
            DatePicker dateInput = new()
            {
                Name = "CreationDate",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(20, 0, 0, 0),
                Width = 146,
                SelectedDateFormat = DatePickerFormat.Short
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
            Container.Children.Add(inputLabel);

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


            coursesList.SelectionChanged += (sender, e) =>
            {
                controls.First().Value.Visibility = Visibility.Visible;
                controls.ElementAt(1).Value.Visibility = Visibility.Visible;

                Course selectedCourse = courses.Where(x => x.Title == coursesList.SelectedItem as string).First();
                foreach(var props in selectedCourse.GetType().GetProperties())
                {
                    if (props.Name != "Id") propsList.Items.Add(props.Name);
                }
            };

            propsList.SelectionChanged += async (sender, e) =>
            {
                inputLabel.Visibility = Visibility.Visible;
                long currentId = coursesMap.FirstOrDefault(x => x.Value == coursesList.SelectedItem.ToString()).Key;
                string currentProp = propsList.SelectedItem.ToString();
                Course currentCourse = await GetCourseById(currentId);
                var currentPropValue = currentCourse.GetType()
                                                       .GetProperty(currentProp)
                                                       .GetValue(currentCourse);

                if(currentPropValue != null && currentProp.Equals("GrantsCertification"))
                {
                    Container.Children.Remove(input);
                    Container.Children.Remove(dateInput);
                    Container.Children.Remove(confirm);
                    Container.Children.Add(grInput);
                    Container.Children.Add(confirm);
                    inputLabel.Content = $"Current {currentProp} value is {currentPropValue}";
                    inputLabel.Foreground = Brushes.Black;
                }
                else if(currentPropValue != null && currentProp.Equals("CreationDate"))
                {
                    Container.Children.Remove(input);
                    Container.Children.Remove(grInput);
                    Container.Children.Remove(confirm);
                    Container.Children.Add(dateInput);
                    Container.Children.Add(confirm);
                    inputLabel.Content = $"Current {currentProp} value is {currentPropValue}";
                    inputLabel.Foreground = Brushes.Black;
                }
                else if(currentPropValue != null && !currentProp.Equals("GrantsCertification"))
                {
                    Container.Children.Remove(grInput);
                    Container.Children.Remove(dateInput);
                    Container.Children.Remove(confirm);
                    Container.Children.Add(input);
                    Container.Children.Add(confirm);
                    inputLabel.Content = $"Current {currentProp} value is {currentPropValue}";
                    inputLabel.Foreground = Brushes.Black;
                }
                else
                {
                    inputLabel.Content = "CURRENT VALUE IS NULL";
                    inputLabel.Foreground = Brushes.Red;
                }
            };

            confirm.Click += async (sender, e) => 
            {
                long currentId = coursesMap.FirstOrDefault(x => x.Value == coursesList.SelectedItem.ToString()).Key;
                string propToUpdate = propsList.SelectedItem.ToString();
                Course oldCourse = await GetCourseById(currentId);
                dynamic currentPropValue = oldCourse.GetType()
                                       .GetProperty(propToUpdate)
                                       .GetValue(oldCourse);
                Course newCourse = new();
                HttpResponseMessage response;

                if (propToUpdate.Equals("GrantsCertification")) oldCourse.GrantsCertification = grInput.IsChecked;
                else oldCourse.GetType().GetProperty(propToUpdate).SetValue(oldCourse, currentPropValue);

                using HttpClient client = new();
                response = await client.DeleteAsync($"https://localhost:44331/api/course/{currentId}");
                response.EnsureSuccessStatusCode();

                foreach(var p in oldCourse.GetType().GetProperties())
                {
                    if(p.Name != "Id") newCourse.GetType().GetProperty(p.Name).SetValue(newCourse, p.GetValue(oldCourse));
                }

                response = await client.PostAsJsonAsync("https://localhost:44331/api/course/", newCourse);
                response.EnsureSuccessStatusCode();
                newCourse = await response.Content.ReadAsAsync<Course>();

                MessageBox.Show($"{propToUpdate} successfully updated, new course Id is {newCourse.Id}");
                Close();
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

        private static async Task<Course> GetCourseById(long id)
        {
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync($"https://localhost:44331/api/course/{id}");
            Course c = await response.Content.ReadAsAsync<Course>();
            response.EnsureSuccessStatusCode();

            return c;
        }
    }
}
