using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Trello
{
    class Program
    {
        static void Main(string[] args)
        {
            SQLiteConnection sqliteConn;
            sqliteConn = CreateConnection();
            User currentUser = new User();
            while (true)
            {
                bool registered = currentUser.Register();
                if (registered)
                {
                    break;
                }

            }
            while (currentUser.Id==0)
            {
                currentUser = currentUser.Login();
            }


            Console.WriteLine(currentUser.ToString());
            Console.ReadLine();
        }

        public static SQLiteConnection CreateConnection()
        {
            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            string path = Path.Combine(Environment.CurrentDirectory, @"..\..\..\", "trello.db");
            string connetionString = "Data Source=" + path + ";";
            sqlite_conn = new SQLiteConnection(connetionString);
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return sqlite_conn;
        }

        //public static void ReadData(SQLiteConnection conn)
        //{
        //    SQLiteDataReader sqlite_datareader;
        //    SQLiteCommand sqlite_cmd;
        //    sqlite_cmd = conn.CreateCommand();
        //    sqlite_cmd.CommandText = "SELECT * from users";

        //    sqlite_datareader = sqlite_cmd.ExecuteReader();
        //    while (sqlite_datareader.Read())
        //    {
        //        string myreader = sqlite_datareader.GetString(0);
        //        Console.WriteLine(myreader);
        //    }
        //    conn.Close();
        //}
    }
}
