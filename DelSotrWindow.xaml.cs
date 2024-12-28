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

namespace BlgFnd
{
    /// <summary>
    /// Логика взаимодействия для DelSotrWindow.xaml
    /// </summary>
    public partial class DelSotrWindow : Window
    {
        private DatabaseConnection dbConnection;
        public DelSotrWindow()
        {
            InitializeComponent();
            dbConnection = new DatabaseConnection();
        }
        public void LoadData()
        {
            var logUs = logUsTB.Text;
            var adminkey = adminkeyTB.Text;
            using (var connection = dbConnection.GetConnection())
            {
                connection.Open();
                string qwery = $"SELECT login, password FROM Sotr WHERE login = 'adminkey' and password = '{adminkey}'";
                using (var command = new NpgsqlCommand(qwery, connection))
                {
                    using (var adapter = new NpgsqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        if (dataTable.Rows.Count == 1)
                        {
                            string wasd2 = $"SELECT * FROM Sotr WHERE login = '{logUs}'";
                            using (var command3 = new NpgsqlCommand(wasd2, connection))
                            {
                                using (var adapter2 = new NpgsqlDataAdapter(command3))
                                {
                                    DataTable dataTable2 = new DataTable();
                                    adapter2.Fill(dataTable2);
                                    if (dataTable2.Rows.Count == 0)
                                    {
                                        MessageBox.Show("Сотрудник с таким логином не обнаружен", "Ой!");
                                    }
                                    else
                                    {
                                        string wasd = $"DELETE FROM Sotr WHERE login = '{logUs}'";
                                        try
                                        {
                                            using (var command2 = new NpgsqlCommand(wasd, connection))
                                            {
                                                command2.ExecuteNonQuery();
                                                MessageBox.Show("Сотрудник удален", "Успешно!");
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.Message, "Ошибка!");
                                        }
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

        private void DelUsB_Click(object sender, RoutedEventArgs e)
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
