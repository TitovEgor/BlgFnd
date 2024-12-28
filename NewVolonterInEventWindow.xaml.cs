using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
    /// Логика взаимодействия для NewVolonterInEventWindow.xaml
    /// </summary>
    public partial class NewVolonterInEventWindow : Window
    {
        private DatabaseConnection dbConnection;
        public NewVolonterInEventWindow()
        {
            InitializeComponent();
            dbConnection = new DatabaseConnection();
            LoadVolonter();
        }
        public class Event
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return Name;  // Это отображаемое значение в ComboBox
            }
        }
        public class Volonter
        {
            public int Id { get; set; }
            public string FullName { get; set; }

            public override string ToString()
            {
                return FullName;  // Это отображаемое значение в ComboBox
            }
        }
        private void LoadEvent()
        {
            DateTime? selectedDate = PlaneOfEventDP.SelectedDate; // Получаем дату из DatePicker
            if (selectedDate.HasValue)
            {
                EventCB.Items.Clear();
                using (var connection = dbConnection.GetConnection())
                {
                    connection.Open();
                    // Запрос на получение плана на дату
                    string query2 = "SELECT e.EventID, e.Name " +
                                    "FROM Event e " +
                                    "JOIN EventVolonter ev ON e.EventID = ev.EventID " +
                                    "JOIN PlanEvent pe ON ev.EventVolonterID = pe.EventVolonterID " +
                                    "JOIN PlanOfEvent poe ON pe.PlanID = poe.PlanID " +
                                    "WHERE poe.PlaneDate = @date " + // Фильтрация по дате плана
                                    "ORDER BY poe.PlanDate ASC";  // Сортировка по дате
                    using (var command = new NpgsqlCommand(query2, connection))
                    {
                        command.Parameters.AddWithValue("@date", selectedDate.Value);
                        try
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var Event = new Event
                                    {
                                        Id = reader.GetInt32(0),
                                        Name = reader.GetString(1)
                                    };

                                    // Добавляем сотрудника в ComboBox
                                    EventCB.Items.Add(Event);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Ошибка при загрузке");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите дату для поиска.", "Предупреждение");
            }            
        }
        private void LoadVolonter()
        {
            VolonterCB.Items.Clear();
            try
            {
                using (var connection = dbConnection.GetConnection())
                {
                    connection.Open();

                    // Запрос на получение плана на дату
                    string query3 = "SELECT Volid, FullName FROM Volonter ";
                    using (var command = new NpgsqlCommand(query3, connection))
                    {
                        try
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var vol = new Volonter
                                    {
                                        Id = reader.GetInt32(0),
                                        FullName = reader.GetString(1)
                                    };
                                    // Добавляем сотрудника в ComboBox
                                    VolonterCB.Items.Add(vol);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Ошибка при загрузке");
                        }
                    }
                }
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message, "Ошибка загрузки сотрудников");
            }
        }
        private void CheckVolInEvent()
        {
            var selectedevent = (Event)EventCB.SelectedItem;
            var selectedvol = (Volonter)VolonterCB.SelectedItem;
            using (var connection = dbConnection.GetConnection())
            {
                connection.Open();

                // Запрос на получение волонтеров на мероприятии
                string query5 = "Select * FROM EventVolonter WHERE eventId = @event AND volonterid = @vol;";
                using (var command = new NpgsqlCommand(query5, connection))
                {
                    command.Parameters.AddWithValue("@event", selectedevent.Id);
                    command.Parameters.AddWithValue("@vol", selectedvol.Id);
                    try
                    {
                        using (var adapter = new NpgsqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            if (dataTable.Rows.Count == 1)
                            {
                                MessageBox.Show("Волонтер уже добавлен на это мероприятие","Ошибка");
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                AddVolInEvent();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка");
                    }
                }
            }
        }
        private void AddVolInEvent()
        {
            var selectedevent = (Event)EventCB.SelectedItem;
            var selectedvol = (Volonter)VolonterCB.SelectedItem;
            using (var connection = dbConnection.GetConnection())
            {
                connection.Open();

                // Запрос на получение плана на дату
                string query4 = "UPDATE EventVolonter Set volonterid = @vol WHERE eventId = @event";
                using (var command = new NpgsqlCommand(query4, connection))
                {
                    command.Parameters.AddWithValue("@event",selectedevent.Id);
                    command.Parameters.AddWithValue("@vol",selectedvol.Id);
                    try
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("Волонтер добавлен на мероприятие!","Успешно");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка");
                    }
                }
            }
        }
        private void BackB_Click(object sender, RoutedEventArgs e)
        {
            NewEventWindow newEventWindow = new NewEventWindow();
            this.Close();
            newEventWindow.Show();
        }

        private void AddB_Click(object sender, RoutedEventArgs e)
        {
            CheckVolInEvent();
        }
        private void PlaneOfEventCB_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadEvent();
        }
    }
}
