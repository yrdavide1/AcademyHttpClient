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
using AcademyHttpClientGUI.SubWindows;

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

        private void Display_Students(object sender, RoutedEventArgs e)
        {
            DisplayStudents displayStudents = new DisplayStudents();
            displayStudents.Show();
        }

        private void Create_Student(object sender, RoutedEventArgs e)
        {
            CreateStudent createStudent = new CreateStudent();
            createStudent.Show();
        }

        private void Delete_Student(object sender, RoutedEventArgs e)
        {
            DeleteStudent deleteStudent = new DeleteStudent();
            deleteStudent.Show();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

}
