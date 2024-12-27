using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace BlgFnd
{
    //Класс DatabaseConnection предназнначен для управления подключением к БД
    public class DatabaseConnection
    {
        private string connectionString = "Server=localhost;Port=5432;Database=BlgFndDB2;User Id=postgres;Password=123";

        public NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}
