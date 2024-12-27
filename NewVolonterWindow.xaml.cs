using Npgsql;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для NewVolonterWindow.xaml
    /// </summary>
    public partial class NewVolonterWindow : Window
    {
        private DatabaseConnection dbConnection;
        public NewVolonterWindow()
        {
            InitializeComponent();
            dbConnection = new DatabaseConnection();
            LoadGender();
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
                            MessageBox.Show(ex.Message, "Ошибка загрузке сотрудников");
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
                string query2 = "INSERT INTO Volonter (fullname, gender, Strenght, Smart) VALUES (@name,@gender,@str,@smrt);";
                using (var command2 = new NpgsqlCommand(query2, connection))
                {
                    command2.Parameters.AddWithValue("@name", FullNameTB.Text);
                    command2.Parameters.AddWithValue("@gender", selectedGender.Id);
                    command2.Parameters.AddWithValue("@str", isStrengthChecked ? (byte)1 : (byte)0);  // Передаем 1/0 для поля BIT
                    command2.Parameters.AddWithValue("@smrt", isSmartChecked ? (byte)1 : (byte)0);     // Передаем 1/0 для поля BIT
                    try
                    {
                        command2.ExecuteNonQuery();
                        MessageBox.Show("Волонтер добавлен","успешно!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка");
                    }
                }
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            CreateVol();
        }

        private void BackB_Click(object sender, RoutedEventArgs e)
        {
            VolonterWindow volonterWindow = new VolonterWindow();
            this.Close();
            volonterWindow.Show();
        }
    }
}
