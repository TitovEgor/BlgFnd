using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlgFnd
{
    /// <summary>
    /// Логика взаимодействия для EditPassWindow.xaml
    /// </summary>
    public partial class EditPassWindow : Window
    {
        private DatabaseConnection dbConnection;
        public EditPassWindow()
        {
            InitializeComponent();
            dbConnection = new DatabaseConnection();
        }

        public void LoadData()
        {
            var logUs = logUsTB.Text;
            var passUs = passUsTB.Text;
            var newpassUs = newpassUsTB.Text;
            var adminkey = adminkeyTB.Text;
            using (var connection = dbConnection.GetConnection())
            {
                connection.Open();
                string qwery = "SELECT login, password FROM Sotr WHERE login = 'adminkey' and password = @adminkey";
                using (var command = new NpgsqlCommand(qwery, connection))
                {
                    command.Parameters.AddWithValue("@adminkey", adminkey);
                    using (var adapter = new NpgsqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        if (dataTable.Rows.Count == 1)
                        {
                            string wasd = $"Update sotr Set password = @newpassUs WHERE login = @logUs and password = @passUs";
                            using (var command2 = new NpgsqlCommand(wasd, connection))
                            {
                                command2.Parameters.AddWithValue("@logUs", logUs);
                                command2.Parameters.AddWithValue("@passUs", passUs);
                                command2.Parameters.AddWithValue("@newpassUs", newpassUs);
                                using (var adapter2 = new NpgsqlDataAdapter(command2))
                                {
                                    try
                                    {
                                        command2.ExecuteNonQuery();
                                        MessageBox.Show("Пароль изменен", "Успешно!");
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message, "Ошибка!");
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("неверный пароль администратора", "Ошибка!");
                        }
                    }
                }
            }
        }
        private void uppassUsB_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void BackB_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }
    }
}
