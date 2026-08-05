using MySql.Data.MySqlClient;

namespace SalaoDeBeleza.Data
{
    /// <summary>
    /// Responsável por fornecer a conexão com o banco de dados MySQL.
    /// </summary>
    public class Database
    {
        private readonly string connectionString =
            "server=localhost;port=3306;database=bdsalaodebeleza;user=root;password=12345678;";

        /// <summary>
        /// Abre e retorna uma conexão com o banco de dados.
        /// </summary>
        public MySqlConnection GetConnection()
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();
            return conn;
        }
    }
}