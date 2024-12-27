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
    /// Логика взаимодействия для NewSotrWindow.xaml
    /// </summary>
    public partial class NewSotrWindow : Window
    {
        private DatabaseConnection dbConnection;
        public NewSotrWindow()
        {
            InitializeComponent();
            dbConnection = new DatabaseConnection();
        }

        public void CheckAdminKey()
        {
            using (var connection = dbConnection.GetConnection())
            {
                connection.Open();
                string qwery = $"SELECT login, password FROM Sotr WHERE login = 'adminkey' and password = @adminkey";
                using (var command = new NpgsqlCommand(qwery, connection))
                {
                    command.Parameters.AddWithValue("@adminkey",adminkeyTB.Text);
                    using (var adapter = new NpgsqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        if (dataTable.Rows.Count == 1)
                        {
                            CheckUnik();
                        }
                        else
                        {
                            MessageBox.Show("неверный пароль администратора", "Ошибка!");
                        }
                    }
                }
            }
        }
        public void CheckUnik()
        {
            using (var connection = dbConnection.GetConnection())
            {
                connection.Open();
                string wasd = $"SELECT login, fullname FROM Sotr WHERE login = @logUs or fullname = @fullnameUs;";
                using (var command2 = new NpgsqlCommand(wasd, connection))
                {
                    command2.Parameters.AddWithValue("@logUs", logUsTB.Text);
                    command2.Parameters.AddWithValue("@fullnameUs", fullnameUsTB.Text);
                    using (var adapter2 = new NpgsqlDataAdapter(command2))
                    {
                        DataTable dataTable2 = new DataTable();
                        adapter2.Fill(dataTable2);
                        if (dataTable2.Rows.Count == 1)
                        {
                            MessageBox.Show("Такой логин или имя уже используется", "Внимание!");
                        }
                        else
                        {
                            CreateSotr();
                        }
                    }
                }
            }
        }
        public void CreateSotr()
        {
            using (var connection = dbConnection.GetConnection())
            {
                connection.Open();
                string xyz = $"INSERT INTO Sotr (login, password, fullname) VALUES (@logUs, @passUs, @fullnameUs);";
                using (var command3 = new NpgsqlCommand(xyz, connection))
                {
                    command3.Parameters.AddWithValue("@logUs", logUsTB.Text);
                    command3.Parameters.AddWithValue("@passUs", passUsTB.Text);
                    command3.Parameters.AddWithValue("@fullnameUs", fullnameUsTB.Text);
                    try
                    {
                        command3.ExecuteNonQuery();
                        MessageBox.Show("Пользователь создан", "Успешно!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка!");
                    }
                }
            }
        }
        private void NewUsB_Click(object sender, RoutedEventArgs e)
        {
            CheckAdminKey();
        }

        private void BackB_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }

        private void DelUsB_Click(object sender, RoutedEventArgs e)
        {
            DelSotrWindow window = new DelSotrWindow();
            this.Close();
            window.Show();
        }
    }
}
