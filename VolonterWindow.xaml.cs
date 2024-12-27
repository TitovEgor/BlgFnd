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
using static BlgFnd.NewVolonterInEventWindow;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlgFnd
{
    /// <summary>
    /// Логика взаимодействия для VolonterWindow.xaml
    /// </summary>
    public partial class VolonterWindow : Window
    {
        private DatabaseConnection dbConnection;
        public VolonterWindow()
        {
            InitializeComponent();
            dbConnection = new DatabaseConnection();
            LoadData();
            LoadGender();
        }
        public string str;
        public string smrt;
        public void LoadData()
        {
            using (var connection = dbConnection.GetConnection())
            {
                connection.Open();
                string qwery = "Select v.volid AS Номер, v.fullname AS ФИО, g.gender AS пол, v.Strenght AS сильный, v.Smart AS умный " +
                    "From volonter v " +
                    "JOIN Gender g ON v.Gender = g.GenderId " +
                    "ORDER BY v.fullname ASC ";
                using (var command = new NpgsqlCommand(qwery, connection))
                {
                    using (var adapter = new NpgsqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        try
                        {
                            adapter.Fill(dataTable);
                            DataGrid.ItemsSource = dataTable.DefaultView;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Ошибка!");
                        }
                    }
                }
            }
        }
        private void LoadGender()
        {
            GenderCB.Items.Clear();
            try
            {
                using (var connection = dbConnection.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM Gender ";
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        try
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var gender = new Gender
                                    {
                                        Id = reader.GetInt32(0),
                                        GenderN = reader.GetString(1)
                                    };

                                    // Добавляем сотрудника в ComboBox
                                    GenderCB.Items.Add(gender);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Ошибка загрузке пола");
                        }
                    }
                }
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message, "Ошибка загрузки");
            }
        }
        public class Gender
        {
            public int Id { get; set; }
            public string GenderN { get; set; }

            public override string ToString()
            {
                return GenderN;  // Это отображаемое значение в ComboBox
            }
        }
        private void CreateVol()
        {
            var selectedGender = (Gender)GenderCB.SelectedItem;
            if (selectedGender == null)
            {
                MessageBox.Show("Пожалуйста, выберите пол.", "Ошибка");
                return;
            }
            bool isStrengthChecked = StrenghtCheck.IsChecked == true;
            bool isSmartChecked = SmartCheck.IsChecked == true;
            using (var connection = dbConnection.GetConnection())
            {
                connection.Open();
                string query2 = $"INSERT INTO Volonter (fullname, gender, Strenght, Smart) VALUES (@name,@gender,{str},{smrt});";
                using (var command2 = new NpgsqlCommand(query2, connection))
                {
                    command2.Parameters.AddWithValue("@name", FullNameTB.Text);
                    command2.Parameters.AddWithValue("@gender", selectedGender.Id);
                    try
                    {
                        command2.ExecuteNonQuery();
                        MessageBox.Show("Волонтер добавлен", "успешно!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка");
                    }
                }
            }
        }      
        private void DeleteVol(int volid)
        {
            using (var connection = dbConnection.GetConnection())
            {
                connection.Open();
                string qwert = "DELETE FROM Volonter WHERE VolId = @volid";
                using (var commanddel = new NpgsqlCommand(qwert,connection))
                {
                    commanddel.Parameters.AddWithValue("@volid",volid);
                    commanddel.ExecuteNonQuery();
                }
            }
        }
        private void UpVol()
        {
            var selectedGender = (Gender)GenderCB.SelectedItem;
            if (selectedGender == null)
            {
                MessageBox.Show("Пожалуйста, выберите пол.", "Ошибка");
                return;
            }
            using (var connection = dbConnection.GetConnection())
            {
                connection.Open();
                string query2 = $"UPDATE Volonter Set fullname=@name, gender=@gender, Strenght={str}, Smart={smrt} WHERE volid = @volid;";
                using (var command2 = new NpgsqlCommand(query2, connection))
                {
                    command2.Parameters.AddWithValue("@volid", Convert.ToInt32(IdTB.Text));
                    command2.Parameters.AddWithValue("@name", FullNameTB.Text);
                    command2.Parameters.AddWithValue("@gender", selectedGender.Id);
                    try
                    {
                        command2.ExecuteNonQuery();
                        MessageBox.Show("Волонтер добавлен", "успешно!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка");
                    }
                }
            }
        }
        private void SearchVol()
        {
            using (var connection = dbConnection.GetConnection())
            {
                connection.Open();
                string query = "Select v.fullname AS ФИО, g.gender AS пол, v.Strenght AS сильный, v.Smart AS умный " +
                    "From volonter v " + 
                    "JOIN Gender g ON v.Gender = g.GenderId " +
                    "Where v.VolId = @volid " + 
                    "ORDER BY ФИО ASC ";
                if (IdTB.Text.Length > 0)
                {
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@volid", Convert.ToInt32(IdTB.Text));
                        using (var adapter = new NpgsqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            try
                            {
                                adapter.Fill(dataTable);
                                DataGrid.ItemsSource = dataTable.DefaultView;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}", "Ошибка!");
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"Введите ID", "Ошибка!");
                }
            }
        }
        private void BackB_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }

        public void AddB_Click(object sender, RoutedEventArgs e)
        {
            if (StrenghtCheck.IsChecked == true)
            {
                str = "B'1'";
            }
            else
            {
                str = "B'0'";
            }
            if (SmartCheck.IsChecked == true)
            {
                smrt = "B'1'";
            }
            else
            {
                smrt = "B'0'";
            }
            CreateVol();
            LoadData();
        }

        private void UpdateB_Click(object sender, RoutedEventArgs e)
        {
            if (StrenghtCheck.IsChecked == true)
            {
                str = "B'1'";
            }
            else
            {
                str = "B'0'";
            }
            if (SmartCheck.IsChecked == true)
            {
                smrt = "B'1'";
            }
            else
            {
                smrt = "B'0'";
            }
            UpVol();
            LoadData();
        }

        private void DeleteB_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedrow = (DataRowView)DataGrid.SelectedItem;
            if (selectedrow != null)
            {
                int volid = Convert.ToInt32(selectedrow["Номер"]);
                DeleteVol(volid);
                LoadData();
            }
            else
            {
                MessageBox.Show("Выберите в списке волонтера, которого хотите удалить", "Внимание");
            }
        }

        private void SearchVolB_Click(object sender, RoutedEventArgs e)
        {
            SearchVol();
            IdTB.Clear();
        }
    }
}
