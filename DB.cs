using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserSwitch
{
    class DB
    {
        private String mysqlUsername;
        private String mysqlPassword;
        private String mysqlHost;
        private String mysqlDatabase;
        private MySqlConnection connection;

        public DB(String mysqlUsername, String mysqlPassword, String mysqlHost, String mysqlDatabase)
        {
            this.mysqlUsername = mysqlUsername;
            this.mysqlPassword = mysqlPassword;
            this.mysqlHost = mysqlHost;
            this.mysqlDatabase = mysqlDatabase;
        }

        private void connectToDB()
        {
            String Connectionstring = @"server=" + mysqlHost + ";userid=" + mysqlUsername + ";password=" + mysqlPassword + ";database=" + mysqlDatabase;
            this.connection = new MySqlConnection(Connectionstring);
        }
        public async void writeStatusToDB()
        {
            connectToDB();

            try
            {
                connection.Open();
                String hostname = System.Environment.MachineName;
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO screen (hostname, last_login) VALUES(?name,?last_login) ON DUPLICATE KEY UPDATE last_login= ?last_login";
                command.Parameters.AddWithValue("?last_login", DateTime.Now);
                command.Parameters.AddWithValue("?name", hostname);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }

            }

        }
    }
}
