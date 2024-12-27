using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlgFnd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void VolonterButton_Click(object sender, RoutedEventArgs e)
        {
            VolonterWindow volonterWindow = new VolonterWindow();
            this.Close();
            volonterWindow.Show();
        }

        private void EventButton_Click(object sender, RoutedEventArgs e)
        {
            EventWindow eventForm = new EventWindow();
            this.Hide();
            eventForm.Show();
        }

        private void EditPassButton_Click(object sender, RoutedEventArgs e)
        {
            EditPassWindow editPassWindow = new EditPassWindow();
            this.Close();
            editPassWindow.Show();
        }
        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow();
            this.Close();
            adminWindow.Show();
        }

        private void DonationButton_Click(object sender, RoutedEventArgs e)
        {
            DonationWindow donationWindow1 = new DonationWindow();
            this.Close();
            donationWindow1.Show();
        }
    }
}