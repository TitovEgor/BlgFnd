using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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
    /// Логика взаимодействия для AutWindow.xaml
    /// </summary>
    public partial class AutWindow : Window
    {
        private DatabaseConnection dbConnection;
        public AutWindow()
        {
            InitializeComponent();
            dbConnection = new DatabaseConnection();
        }

        private void enterB_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            var logUs = logUsTB.Text;
            var passUs = passUsTB.Text;
            using (var connection = dbConnection.GetConnection())
            {
                connection.Open();
                string qwery = $"SELECT login, password FROM Sotr WHERE login = @logUs and password = @passUs";
                using (var command = new NpgsqlCommand(qwery, connection))
                {
                    command.Parameters.AddWithValue("@logUs",logUs);
                    command.Parameters.AddWithValue("@passUs",passUs);
                    using (var adapter = new NpgsqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        if (dataTable.Rows.Count == 1)
                        {
                            MessageBox.Show("Вы успешно вошли", "Успешно!");
                            this.Close();
                            mainWindow.Show();
                        }
                        else
                        {
                            MessageBox.Show("Такого аккаунта не существует", "Ошибка!");
                        }
                    }
                }
            }
        }
    }
}
