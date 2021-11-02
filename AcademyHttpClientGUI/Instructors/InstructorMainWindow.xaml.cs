using System;
using System.Collections.Generic;
using System.Linq;
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

namespace AcademyHttpClientGUI.Instructors
{
    public class Instrcutor : Person
    {
        public bool? IsContractor { get; set; }
        public decimal PayRate { get; set; }
    }

    /// <summary>
    /// Logica di interazione per InstructorMainWindow.xaml
    /// </summary>
    public partial class InstructorMainWindow : Window
    {
        public InstructorMainWindow()
        {
            InitializeComponent();
            Onload();
        }

        private void Onload()
        {
            int index = 0;
            Dictionary<string, string> actions = new()
            {
                { "Default", "ACTIONS" },
                { "Display", "Display instructors" },
                { "Create", "Add an instructor" },
                { "Delete", "Remove an instructor" },
                { "Modify", "Modify an instructor's property" }
            };
            TextBlock actionInfo = new()
            {
                Text = "Select an action to perform",
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 15,
                Margin = new Thickness(0, 20, 0, 0)
            };
            ComboBox actionList = new()
            {
                Width = 200,
                Margin = new Thickness(20, 20, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                Name = "ActionList"
            };

            foreach (KeyValuePair<string, string> action in actions)
            {
                actionList.Items.Insert(index, action.Value);
                index++;
            }

            Container.Children.Add(actionInfo);
            Container.Children.Add(actionList);
            actionList.SelectedIndex = 0;

            actionList.SelectionChanged += (sender, e) =>
            {
                string actionTag = actions.ElementAt(actionList.SelectedIndex).Key;
                //MessageBox.Show(actionTag);
                string objToInstantiate = $"AcademyHttpClientGUI.Instructors.SubWindows." +
                                            $"{actionTag}, AcademyHttpClientGUI";
                Type objType = Type.GetType(objToInstantiate);
                dynamic instance = Activator.CreateInstance(objType);
                instance.Show();
            };
        }
    }
}
