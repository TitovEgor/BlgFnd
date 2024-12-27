using Npgsql;
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
    /// Логика взаимодействия для NewOrgWindow.xaml
    /// </summary>
    public partial class NewOrgWindow : Window
    {
        private DatabaseConnection dbConnection;
        public NewOrgWindow()
        {
            InitializeComponent();
            dbConnection = new DatabaseConnection();
        }

        private void AddB_Click(object sender, RoutedEventArgs e)
        {
            using (var connection = dbConnection.GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO Organiz (Name) VALUES (@name);";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", NameOrgTB.Text);
                    try
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("организация добавлена!", "Успешно!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message,"Ошибка!");
                    }
                }
            }
        }

        private void BackB_Click(object sender, RoutedEventArgs e)
        {
            DonationWindow d = new DonationWindow();
            this.Close();
            d.Show();
        }
    }
}
