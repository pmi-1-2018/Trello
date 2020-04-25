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
            Console.WriteLine("### TRELLO ###");
            SQLiteConnection sqliteConn;
            sqliteConn = CreateConnection();
            User currentUser = new User();

            bool knownKeyPressed = false;
            do
            {
                Console.WriteLine("Press 1 to sign up; Press 2 to login;");
                ConsoleKeyInfo keyReaded = Console.ReadKey();
                Console.WriteLine();
                switch (keyReaded.Key)
                {
                    case ConsoleKey.D1: //Number 1 Key
                        while (true)
                        {
                            bool registered = currentUser.Register();
                            if (registered)
                            {
                                break;
                            }
                        }
                        while (currentUser.Id == 0)
                        {
                            currentUser = currentUser.Login();
                        }
                        knownKeyPressed = true;
                        break;

                    case ConsoleKey.D2: //Number 2 Key
                        while (currentUser.Id == 0)
                        {
                            currentUser = currentUser.Login();
                        }
                        knownKeyPressed = true;
                        break;

                    default: //Not known key pressed
                        Console.WriteLine("Wrong key, please try again.");
                        knownKeyPressed = false;
                        break;
                }
            } while (!knownKeyPressed);

            while (true)
            {
                BoardMenu(currentUser);

            }
            

            
            Console.WriteLine(currentUser.ToString());
            Console.WriteLine("Bye, bye");
            Console.ReadLine();
        }

        public static void BoardMenu(User currentUser)
        {
            bool knownKeyPressed = false;
            do
            {
                Console.WriteLine("Press 1 to create board, Press 2 to see your boards");
                ConsoleKeyInfo keyReaded = Console.ReadKey();
                
                switch (keyReaded.Key)
                {

                    case ConsoleKey.D1:
                        while (true)
                        {
                            bool created = currentUser.CreateBoard();
                            if (created)
                            {
                                break;
                            }
                        }
                        knownKeyPressed = true;
                        break;
                    case ConsoleKey.D2:
                        currentUser.ShowBoards();
                        break;
                }

            } while (!knownKeyPressed);
        }


        public static SQLiteConnection CreateConnection()
        {
            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            string path = Path.Combine(Environment.CurrentDirectory, @"../../../", "trello.db");
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
    }
}
