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
using System.Xml.Linq;

namespace BlgFnd
{
    /// <summary>
    /// Логика взаимодействия для NewEventWindow.xaml
    /// </summary>
    public partial class NewEventWindow : Window
    {
        private DatabaseConnection dbConnection;
        public NewEventWindow()
        {
            InitializeComponent();
            dbConnection = new DatabaseConnection();
            LoadSotr();
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
        public class Plane
        {
            public int Id { get; set; }
            public DateTime Planedate { get; set; }

            public override string ToString()
            {
                return Convert.ToString(Planedate);  // Это отображаемое значение в ComboBox
            }
        }
        public void CheckUnik()
        {
            if (DateTime.TryParse(DateTB.Text, out DateTime parsedDate))
            {
                using (var connection = dbConnection.GetConnection())
                {
                    connection.Open();
                    string qwery = "SELECT poe.PlaneDate, e.time_start, e.time_end " +
                        "FROM PlanEvent pe " +
                        "JOIN PlanOfEvent poe ON pe.PlanID = poe.PlanID " +
                        "JOIN EventVolonter ev ON pe.EventVolonterID = ev.EventVolonterID " +
                        "JOIN Event e ON ev.EventID = e.EventID " +
                        "WHERE poe.Planedate = @date AND e.time_end > @time_start AND e.time_start < @time_end;";
                    using (var command = new NpgsqlCommand(qwery, connection))
                    {
                        command.Parameters.AddWithValue("@name", nameTB.Text);
                        command.Parameters.AddWithValue("@time_start", TimeSpan.Parse(time_startTB.Text));
                        command.Parameters.AddWithValue("@time_end", TimeSpan.Parse(time_endTB.Text));
                        command.Parameters.AddWithValue("@time_end", time_endTB.Text);
                        command.Parameters.AddWithValue("@date", parsedDate);
                        using (var adapter = new NpgsqlDataAdapter(command))
                        {
                            try
                            {
                                DataTable dataTable = new DataTable();
                                adapter.Fill(dataTable);
                                if (dataTable.Rows.Count == 1)
                                {
                                    try
                                    {
                                        DialogResult result = MessageBox.Show("В это время уже проводится мероприятие. Хотите продолжить?", "Ошибка!", MessageBoxButton.YesNo);
                                        if (result)
                                        {
                                            command.ExecuteNonQuery();
                                            MessageBox.Show("Данные успешно записаны.", "Информация");
                                        }
                                        else
                                        {
                                            MessageBox.Show("Операция отменена пользователем.", "Информация");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message, "Ошибка!");
                                    }
                                }
                                else
                                {
                                    CreateEvent();
                                }

                            }
                            catch (Exception ex2)
                            {
                                MessageBox.Show(ex2.Message, "Ошибка!");
                            }
                        }
                    }

                }
            } 
        }
        public void CreateEvent()
        {
            var selectedSotr = (Sotr)SotridCB.SelectedItem; 
            if (selectedSotr == null)
            {
                MessageBox.Show("Пожалуйста, выберите сотрудника.", "Ошибка");
                return;
            }
            if (DateTime.TryParse(DateTB.Text, out DateTime parsedDate))
            {
                using (var connection = dbConnection.GetConnection())
                {
                    connection.Open();

                    // Вставка нового мероприятия в таблицу event
                    string qwery2 = "INSERT INTO event (name, description, time_start, time_end, sotrid) " +
                                    "VALUES (@name, @description, @time_start, @time_end, @sotrid) " +
                                    "RETURNING eventid;";
                    try
                    {
                        using (var command = new NpgsqlCommand(qwery2, connection))
                        {
                            command.Parameters.AddWithValue("@name", nameTB.Text);
                            command.Parameters.AddWithValue("@description", DescriptionTB.Text);
                            command.Parameters.AddWithValue("@time_start", TimeSpan.Parse(time_startTB.Text));
                            command.Parameters.AddWithValue("@time_end", TimeSpan.Parse(time_endTB.Text));
                            command.Parameters.AddWithValue("@sotrid", selectedSotr.Id);

                            // Получаем EventID для нового мероприятия
                            int eventId = (int)command.ExecuteScalar(); // Возвращаем значение последнего вставленного eventid

                            // Вставка в таблицу eventvolonter
                            string insertEventVolonterQuery = "INSERT INTO eventvolonter (eventid) VALUES (@eventid) " +
                                                              "RETURNING eventvolonterid;";
                            using (var volonterCommand = new NpgsqlCommand(insertEventVolonterQuery, connection))
                            {
                                volonterCommand.Parameters.AddWithValue("@eventid", eventId);

                                // Получаем EventVolonterID для вставки в таблицу planevent
                                int eventVolonterId = (int)volonterCommand.ExecuteScalar();

                                // Обновление таблицы planevent с добавлением EventVolonterID
                                string updatePlaneventQuery = "UPDATE planevent " +
                                                              "SET EventVolonterID = @eventVolonterId " +
                                                              "WHERE PlanID = (SELECT PlanID FROM planofevent WHERE PlaneDate = @date LIMIT 1);";

                                using (var updateCommand = new NpgsqlCommand(updatePlaneventQuery, connection))
                                {
                                    updateCommand.Parameters.AddWithValue("@eventVolonterId", eventVolonterId);
                                    updateCommand.Parameters.AddWithValue("@date", parsedDate);

                                    updateCommand.ExecuteNonQuery(); // Обновляем planevent
                                }
                            }

                            MessageBox.Show("Мероприятие добавлено успешно!", "Успех!");
                        }
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show(ex2.Message, "Ошибка при добавлении мероприятия!");
                    }
                }
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            CheckUnik();
        }

        private void BackB_Click(object sender, RoutedEventArgs e)
        {
            EventWindow eventWindow = new EventWindow();
            this.Close();
            eventWindow.Show();
        }

        private void AddVolInEventButton_Click(object sender, RoutedEventArgs e)
        {
            NewVolonterInEventWindow newVolonterInEventWindow = new NewVolonterInEventWindow();
            this.Close();
            newVolonterInEventWindow.Show();
        }
    }
}
