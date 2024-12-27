using Npgsql;
using System;
using System.Collections.Generic;
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
            LoadPlans();
            LoadVolonter();
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

        private void LoadPlans()
        {
            PlaneOfEventCB.Items.Clear();
            try
            {
                using (var connection = dbConnection.GetConnection())
                {
                    connection.Open();

                    // Запрос на получение плана на дату
                    string query = "SELECT PlanID, PlaneDate FROM PlanOfEvent ";  //таблица PlanOdEvent с полями Planid и PlaneDate
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        try
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var Plane = new Plane
                                    {
                                        Id = reader.GetInt32(0),
                                        Planedate = reader.GetDateTime(1)
                                    };

                                    // Добавляем сотрудника в ComboBox
                                    PlaneOfEventCB.Items.Add(Plane);
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
        private void LoadEvent()
        {
            var selectedPlan = (Plane)PlaneOfEventCB.SelectedItem;
            if (selectedPlan == null)
            {
                MessageBox.Show("Пожалуйста, выберите сотрудника.", "Ошибка");
                return;
            }
            EventCB.Items.Clear();
            try
            {
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
                                    "ORDER BY e.Name";  // Сортировка по имени события
                    using (var command = new NpgsqlCommand(query2, connection))
                    {
                        command.Parameters.AddWithValue("@date",selectedPlan.Planedate);
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
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message, "Ошибка загрузки сотрудников");
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
            AddVolInEvent();
        }

        private void PlaneOfEventCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadEvent();
        }
    }
}
