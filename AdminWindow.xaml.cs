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

namespace BlgFnd
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
        }

        private void NewSotrButton_Click(object sender, RoutedEventArgs e)
        {
            NewSotrWindow newSotrWindow = new NewSotrWindow();
            this.Close();
            newSotrWindow.Show();
        }

        private void DelSotrButton_Click(object sender, RoutedEventArgs e)
        {
            DelSotrWindow delSotrWindow = new DelSotrWindow();
            this.Close();
            delSotrWindow.Show();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }
    }
}
