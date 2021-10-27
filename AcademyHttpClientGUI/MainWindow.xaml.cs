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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AcademyHttpClientGUI
{
    public class Person
    {
        public long Id { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
    public class Student : Person
    {
        public bool? IsEmployee { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Display_Students(object sender, RoutedEventArgs e)
        {
            Message.Text = "";
            Message.Visibility = Visibility.Hidden;
            System.Threading.Thread.Sleep(500);

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

        private async void Create_Student(object sender, RoutedEventArgs e)
        {
            #region Student Input
            Student studentData = new Student();
            studentData.Firstname = FirstnameInput.Text;
            studentData.Lastname = LastnameInput.Text;

            string date = DateOfBirthInput.Text;
            if (IsValidDate(date)) studentData.DateOfBirth = DateOfBirthInput.Text;

            studentData.Address = AddressInput.Text;
            studentData.City = CityInput.Text;

            if (IsValidEmail(EmailInput.Text)) studentData.Email = EmailInput.Text;

            if (IsValidPhoneNumber(PhoneNumberInput.Text)) studentData.PhoneNumber = PhoneNumberInput.Text;

            studentData.IsEmployee = IsEmployeeInput.IsChecked;
            #endregion

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("https://localhost:44331/api/student/", studentData);
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    response = await client.GetAsync($"https://localhost:44331/api/student/name?fullname={studentData.Firstname}%20{studentData.Lastname}");
                    response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                        List<Student> created = await response.Content.ReadAsAsync<List<Student>>();
                        if (created.Any()) MessageBox.Show($"Student successfully created with ID {created.First().Id}");
                    }
                }
            }
        }

        #region CreateStudent checks
        private static bool IsValidPhoneNumber(string pn)
        {
            if (pn.Length == 10)
            {
                var isNumeric = long.TryParse(pn, out _);
                if (isNumeric)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsValidDate(string date)
        {
            if (date.Split('/').Length == 3)
            {
                if (date.Split('/')[0].Length == 4
                            && date.Split('/')[1].Length == 2
                            && date.Split('/')[2].Length == 2)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsValidEmail(string email)
        {
            if (email.Trim().EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValidChar(string str, out char c)
        {
            if (str.Length == 1)
            {
                c = str.ToLower()[0];
                return true;
            }
            c = '.';
            return false;
        }
        #endregion
    }

}
