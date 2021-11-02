using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using AcademyHttpClientGUI.Courses.SubWindows;


namespace AcademyHttpClientGUI.Courses
{
    public class Area
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    public class Course
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int Duration { get; set; }
        public decimal BasePrice { get; set; }
        public string? Syllabus { get; set; }
        public int Level { get; set; }
        public long AreaId { get; set; }
        public bool? GrantsCertification { get; set; }
        public string? CreationDate { get; set; }
    }

    /// <summary>
    /// Logica di interazione per CoursesMainWindow.xaml
    /// </summary>
    /// 
    public partial class CoursesMainWindow : Window
    {
        List<object> windowsObjs = new();
        string? windowToShow;

        public CoursesMainWindow()
        {
            InitializeComponent();
            windowsObjs.Add(new Display());
            windowsObjs.Add(new Create());
            windowsObjs.Add(new Delete());
            windowsObjs.Add(new Modify());
        }

        private void ActionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!Default.IsSelected)
            {
                string objectToInstantiate = $"AcademyHttpClientGUI.Courses.SubWindows." +
                                                $"{((ComboBoxItem)CoursesActions.SelectedItem).Tag}, " +
                                                $"AcademyHttpClientGUI";
                Type objType = Type.GetType(objectToInstantiate);
                dynamic test = Activator.CreateInstance(objType);
                test.Show();
            }
        }
    }
}
