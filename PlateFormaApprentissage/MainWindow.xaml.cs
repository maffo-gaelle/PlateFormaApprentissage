using PlateFormaApprentissage.Models;
using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlateFormaApprentissage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Course> Courses { get; set; }
        public ObservableCollection<User> Users { get; set; }
        public MainWindow()
        {
            Courses = new ObservableCollection<Course>(App.context.Courses);
            Users = new ObservableCollection<User>(App.context.Users);

            foreach (var item in Courses)
            {
                Console.WriteLine(item.Title);
            }

            foreach (var item in Users)
            {
                Console.WriteLine(item.Email);
            }

            InitializeComponent();
        }
    }
}
