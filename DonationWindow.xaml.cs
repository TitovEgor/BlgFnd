using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
    /// Логика взаимодействия для DonationWindow.xaml
    /// </summary>
    public partial class DonationWindow : Window
    {
        private DatabaseConnection dbConnection;
        public DonationWindow()
        {
            InitializeComponent();
            dbConnection = new DatabaseConnection();
            LoadSotr();
            LoadOrg();
            LoadData();
        }
        private void LoadSotr()
        {
            SotridCB.Items.Clear();

            try
            {
                using (var connection = dbConnection.GetConnection())
                {
                    connection.Open();

                    // Запрос на получение всех сотрудников
                    string query = "SELECT Sotrid, fullname FROM Sotr WHERE login != 'adminkey'";  //  таблица sotr с полями id и fullname
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        try
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var Sotr = new Sotr
                                    {
                                        Id = reader.GetInt32(0),
                                        Fullname = reader.GetString(1)
                                    };

                                    // Добавляем сотрудника в ComboBox
                                    SotridCB.Items.Add(Sotr);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Ошибка загрузке сотрудников");
                        }
                    }
                }
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message, "Ошибка загрузки сотрудников");
            }
        }
        public class Sotr
        {
            public int Id { get; set; }
            public string Fullname { get; set; }

            public override string ToString()
            {
                return Fullname;  // Это отображаемое значение в ComboBox
            }
        }
        private void LoadOrg()
        {
            OrganizationCB.Items.Clear();
            try
            {
                using (var connection = dbConnection.GetConnection())
                {
                    connection.Open();
                    // Запрос на получение всех сотрудников
                    string query2 = "SELECT * FROM Organiz; ";  //  таблица Organiz с полями id и name
                    using (var command = new NpgsqlCommand(query2, connection))
                    {
                        try
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var organiz = new Organiz
                                    {
                                        Id = reader.GetInt32(0),
                                        Name = reader.GetString(1)
                                    };

                                    // Добавляем сотрудника в ComboBox
                                    OrganizationCB.Items.Add(organiz);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Ошибка загрузки");
                        }
                    }
                }
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message, "Ошибка загрузки сотрудников");
            }
        }
        public class Organiz
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return Name;  // Это отображаемое значение в ComboBox
            }
        }
        private void AddB_Click(object sender, RoutedEventArgs e)
        {
            var selectedSotr = (Sotr)SotridCB.SelectedItem;
            var selectedOrg = (Organiz)OrganizationCB.SelectedItem; 
            if (selectedSotr == null)
            {
                MessageBox.Show("Пожалуйста, выберите сотрудника.", "Ошибка");
                return;
            }
            using (var connection = dbConnection.GetConnection())
            {
                connection.Open();
                string query3 = "INSERT INTO Donation (donatsize, dondate, Sotrudnik, Organization) VALUES (@donatsize,@dondate,@sotr,@org);";
                using (var command3 = new NpgsqlCommand(query3, connection))
                {
                    command3.Parameters.AddWithValue("@donatsize",Convert.ToInt32(DonationSizeTB.Text));
                    command3.Parameters.AddWithValue("@dondate",Convert.ToDateTime(DonDateTB.Text));
                    command3.Parameters.AddWithValue("@sotr", selectedSotr.Id);
                    command3.Parameters.AddWithValue("@org", selectedOrg.Id);
                    try
                    {
                        command3.ExecuteNonQuery();
                        MessageBox.Show("Добавлено!","Успешно!");
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}", "Ошибка!");
                    }
                }
            }
        }
        public void LoadData() //загрузка данных в таблицу при открытии окна
        {
            using (var connection = dbConnection.GetConnection())
            {
                connection.Open();
                string qwery = "SELECT d.DonatSize AS сумма, d.DonDate AS дата, s.Fullname AS сотрудник, o.Name As организация " +
                    "FROM Donation d " +
                    "JOIN Sotr s ON d.Sotrudnik = s.SotrId " +
                    "JOIN Organiz o ON d.Organization = o.OrgId " +
                    "ORDER BY d.DonatSize;";
                using (var command = new NpgsqlCommand(qwery, connection))
                {
                    using (var adapter = new NpgsqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        try
                        {
                            DataGrid.ItemsSource = dataTable.DefaultView;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}", "Ошибка!");
                        }
                    }
                }
            }
        }
        private void AddOrgB_Click(object sender, RoutedEventArgs e)
        {
            NewOrgWindow newOrgWindow = new NewOrgWindow();
            this.Close();
            newOrgWindow.Show();
        }

        private void BackB_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }
    }
}
