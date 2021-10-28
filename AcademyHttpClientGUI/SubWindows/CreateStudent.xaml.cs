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
    /// Logica di interazione per CreateStudent.xaml
    /// </summary>
    public partial class CreateStudent : Window
    {
        public CreateStudent()
        {
            InitializeComponent();
        }


        private async void CreateOnclick(object sender, RoutedEventArgs e)
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
