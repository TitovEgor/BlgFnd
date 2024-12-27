using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
using System.ComponentModel.Design;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlgFnd
{
    /// <summary>
    /// Логика взаимодействия для NewPlanOfEventWindow.xaml
    /// </summary>
    public partial class NewPlanOfEventWindow : Window
    {
        private DatabaseConnection dbConnection;
        public NewPlanOfEventWindow()
        {
            InitializeComponent();
            dbConnection = new DatabaseConnection();
            LoadSotr();
        }

        private void BackB_Click(object sender, RoutedEventArgs e)
        {
            EventWindow eventWindow = new EventWindow();
            this.Close();
            eventWindow.Show();
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
        private void CreateB_Click(object sender, RoutedEventArgs e)
        {
            var selectedSotr = (Sotr)SotridCB.SelectedItem;
            if (selectedSotr == null)
            {
                MessageBox.Show("Пожалуйста, выберите сотрудника.", "Ошибка");
                return;
            }
            if (DateTime.TryParse(PlaneDateTB.Text, out DateTime parsedDate))
            {
                using (var connection = dbConnection.GetConnection())
                {
                    connection.Open();
                    string wasd = "SELECT * FROM PlanOfEvent WHERE PlaneDate = @date";
                    using (var command = new NpgsqlCommand(wasd, connection))
                    {
                        command.Parameters.AddWithValue("@date", Convert.ToDateTime(parsedDate));
                        try
                        {
                            using (var adapter = new NpgsqlDataAdapter(command))
                            {
                                DataTable dataTable = new DataTable();
                                adapter.Fill(dataTable);
                                if (dataTable.Rows.Count == 1)
                                {
                                    command.ExecuteNonQuery();
                                    MessageBox.Show("План мероприятий на эту дату уже соществует", "Внимание!");
                                }
                                else
                                {
                                    string qwery = "INSERT INTO planofevent (planedate, sotrudnik) VALUES (@date,@sotr);";
                                    using (var command2 = new NpgsqlCommand(qwery, connection))
                                    {
                                        command2.Parameters.AddWithValue("@date", parsedDate);
                                        command2.Parameters.AddWithValue("@sotr", selectedSotr.Id);
                                        try
                                        {
                                            command2.ExecuteNonQuery();
                                            MessageBox.Show("Планм ероприятий на эту дату создан", "Успешно!");
                                        }
                                        catch (Exception ex2)
                                        {
                                            MessageBox.Show(ex2.Message, "Ошибка! 2");
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Ошибка! 1");
                        }
                    }
                }
            }
                
        }
    }
}
