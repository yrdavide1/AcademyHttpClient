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
using AcademyHttpClientGUI.SubWindows;

namespace AcademyHttpClientGUI.SubWindows.UpdateStudent
{
    //public static class Extensions
    //{
    //    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self) => self.Select((item, index) => (item, index));
    //}

    /// <summary>
    /// Logica di interazione per DropdownList.xaml
    /// </summary>
    public partial class DropdownList : Window
    {
        public DropdownList()
        {
            InitializeComponent();
            Onload();
        }

        private async void Onload()
        {
            bool x = false;
            var students = await GetStudents();

            foreach (Student s in students)
            {
                IDs.Items.Add(s.Id);
                if (!x)
                {
                    foreach (var p in s.GetType().GetProperties())
                    {
                        if (p.Name != "Id") Fields.Items.Add(p.Name);
                        x = true;
                    }
                }
            }
        }

        private static async Task<List<Student>> GetStudents()
        {
            List<Student> students = new();
            try
            {
                using HttpClient client = new();
                var response = await client.GetAsync("https://localhost:44331/api/student/name");
                response.EnsureSuccessStatusCode();

                students = await response.Content.ReadAsAsync<List<Student>>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source);
            }
            return students;
        }

        private async void Confirm(object sender, RoutedEventArgs e)
        {
            Student oldStudent = await GetStudentByIdAsync($"https://localhost:44331/api/student/{IDs.SelectedItem}");
            Student newStudent = new();
            long idToUpdate = oldStudent.Id;
            string? propToUpdate = Fields.SelectedItem.ToString();
            HttpResponseMessage response;

            if (propToUpdate == "IsEmployee") oldStudent.IsEmployee = IsEmployeeCheckBox.IsChecked;
            else oldStudent.GetType().GetProperty(propToUpdate).SetValue(oldStudent, PropInput.Text);

            using HttpClient client = new();
            response = await client.DeleteAsync($"https://localhost:44331/api/student/{idToUpdate}");
            response.EnsureSuccessStatusCode();

            foreach (var p in oldStudent.GetType().GetProperties())
            {
                if (p.Name != "Id") newStudent.GetType().GetProperty(p.Name).SetValue(newStudent, p.GetValue(oldStudent));
            }

            response = await client.PostAsJsonAsync("https://localhost:44331/api/student", newStudent);
            response.EnsureSuccessStatusCode();
            newStudent = await response.Content.ReadAsAsync<Student>();

            MessageBox.Show($"{propToUpdate} successfully updated, new ID is {newStudent.Id}");
            Close();
        }

        private static async Task<Student> GetStudentByIdAsync(string fullnamePath)
        {
            using HttpClient client = new();
            Student student = new();
            HttpResponseMessage response = await client.GetAsync(fullnamePath);
            student = await response.Content.ReadAsAsync<Student>();
            response.EnsureSuccessStatusCode();

            return student;
        }

        private async void FieldChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!FieldsDefaultItem.IsSelected)
            {
                PropInputLabel.Visibility = Visibility.Visible;
                PropInputLabel.Text = $"{Fields.SelectedItem} value";

                if (Fields.SelectedItem.ToString() == "IsEmployee")
                {
                    IsEmployeeCheckBox.Visibility = Visibility.Visible;
                    PropInput.Visibility = Visibility.Hidden;
                }
                else
                {
                    PropInput.Visibility = Visibility.Visible;
                    IsEmployeeCheckBox.Visibility = Visibility.Hidden;
                }
            }

            if (!IDsDefaultItem.IsSelected && !FieldsDefaultItem.IsSelected)
            {
                var s = await GetStudentByIdAsync($"https://localhost:44331/api/student/{IDs.SelectedItem}");
                CurrentValue.Visibility = Visibility.Visible;
                CurrentValue.Text = $"Current {Fields.SelectedItem} value {s.GetType().GetProperty(Fields.SelectedItem.ToString()).GetValue(s)}";
            }
        }

    }
}
