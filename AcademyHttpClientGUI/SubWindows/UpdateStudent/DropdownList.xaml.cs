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
                        Fields.Items.Add(p.Name);
                        x = true;
                    }
                }
            }
        }

        private async Task<List<Student>> GetStudents()
        {
            List<Student> students = new List<Student>();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync("https://localhost:44331/api/student/name");
                    response.EnsureSuccessStatusCode();

                    students = await response.Content.ReadAsAsync<List<Student>>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source);
            }
            return students;
        }

        private async void Confirm(object sender, RoutedEventArgs e)
        {
            Student studentToUpdate = await GetStudentByIdAsync($"https://localhost:44331/api/student/{IDs.SelectedItem}");
            long idToUpdate = studentToUpdate.Id;
            string? propToUpdate = Fields.SelectedItem.ToString();

            if(propToUpdate != null) PropInputLabel.Text += $"{studentToUpdate.GetType().GetProperty(propToUpdate)}";
            

        }

        private static async Task<Student> GetStudentByIdAsync(string fullnamePath)
        {
            using (HttpClient client = new HttpClient())
            {
                Student student = new Student();
                HttpResponseMessage response = await client.GetAsync(fullnamePath);
                student = await response.Content.ReadAsAsync<Student>();
                response.EnsureSuccessStatusCode();

                return student;
            }
        }

        private void FieldChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!FieldsDefaultItem.IsSelected)
            {
                PropInput.Visibility = Visibility.Visible;
                PropInputLabel.Visibility = Visibility.Visible;
                PropInputLabel.Text = $"{Fields.SelectedItem} value ";
            }
        }
    }
}
