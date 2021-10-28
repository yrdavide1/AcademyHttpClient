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

namespace AcademyHttpClientGUI.SubWindows
{
    /// <summary>
    /// Logica di interazione per DisplayStudents.xaml
    /// </summary>
    public partial class DisplayStudents : Window
    {
        public DisplayStudents()
        {
            InitializeComponent();
            Onload();
        }

        private async void Onload()
        {
            Message.Text = "";
            try
            {
                List<Student> students = new List<Student>();
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync("https://localhost:44331/api/student/name");
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        students = await response.Content.ReadAsAsync<List<Student>>();
                        if (students.Any())
                        {
                            Message.Visibility = Visibility.Visible;
                            foreach (Student student in students)
                            {
                                Message.Text += $"Id: {student.Id}\nFirstname: {student.Firstname}\n" +
                                                    $"Lastname: {student.Lastname}\nDate of Birth: {student.DateOfBirth}\n" +
                                                    $"Address: {student.Address}\nCity: {student.City}\n" +
                                                    $"Email: {student.Email}\nPhone number: {student.PhoneNumber}\n" +
                                                    $"Is Employee: {student.IsEmployee}\n" +
                                                    $"--------------\n";
                                //MessageBox.Show($"Id: {student.Id}\nFirstname: {student.Firstname}\n" +
                                //                    $"Lastname: {student.Lastname}\nDate of Birth: {student.DateOfBirth}\n" +
                                //                    $"Address: {student.Address}\nCity: {student.City}\n" +
                                //                    $"Email: {student.Email}\nPhone number: {student.PhoneNumber}\n");
                            }
                        }
                    }
                    else
                    {
                        Message.Text = $"Server error code {response.StatusCode}";
                    }
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
