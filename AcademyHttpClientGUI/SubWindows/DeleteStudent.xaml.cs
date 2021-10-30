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
    /// Logica di interazione per DeleteStudent.xaml
    /// </summary>
    public partial class DeleteStudent : Window
    {
        public DeleteStudent()
        {
            InitializeComponent();
        }

        private async void Delete(object sender, RoutedEventArgs e)
        {
            long.TryParse(ID.Text, out long x);
            long studentId = IsValidId(x) ? x : -1;

            if(studentId != -1)
            {
                using HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.DeleteAsync($"https://localhost:44331/api/student/{studentId}");
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Student with ID {studentId} successfully deleted!");
                    Close();
                }
            }
        }

        private static bool IsValidId(long id)
        {
            if (id > 0) return true;
            return false;
        }
    }
}
