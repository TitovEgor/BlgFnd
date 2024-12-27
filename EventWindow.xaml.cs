using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
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
using Npgsql;
using Xceed.Wpf.Toolkit;

namespace BlgFnd
{
    /// <summary>
    /// Логика взаимодействия для EventWindow.xaml
    /// </summary>
    public partial class EventWindow : Window
    {
        private DatabaseConnection dbConnection;
        public EventWindow()
        {
            InitializeComponent();
            dbConnection = new DatabaseConnection();
            LoadData();
        }
        public void LoadData() //загрузка данных в таблицу при открытии окна
        {
            using (var connection = dbConnection.GetConnection())
            {
                connection.Open();
                string qwery = "SELECT e.name AS название, e.description AS описание, array_to_string(array_agg(DISTINCT v.fullname), E'\n') AS Участники, poe.PlaneDate AS дата " +
                    "FROM event e " +
                    "LEFT JOIN eventvolonter ev ON e.eventid = ev.eventid " +
                    "LEFT JOIN volonter v ON ev.volonterid = v.volid " +
                    "LEFT JOIN  planevent pe ON ev.eventvolonterid = pe.eventvolonterid " +
                    "LEFT JOIN planofevent poe ON pe.planid = poe.planid " +
                    "GROUP BY e.name, e.description, poe.PlaneDate " +
                    "ORDER BY e.name;";
                try
                {
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
                            catch (Exception ex2)
                            {
                                System.Windows.MessageBox.Show(ex2.Message, "Ошибка!");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Ошибка!");
                }
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NewEventWindow newEventWindow = new NewEventWindow();
            this.Close();
            newEventWindow.Show();
        }

        private void BackB_Click(object sender, RoutedEventArgs e) //вернуться в меню
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }

        private void SqlB_Click(object sender, RoutedEventArgs e) //создать план мероприятий
        {
            NewPlanOfEventWindow newPlanOfEventWindow = new NewPlanOfEventWindow();
            this.Close();
            newPlanOfEventWindow.Show();
        }
    }
}
