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
                            currentUser = currentUser.AutoLogin();
                            //currentUser = currentUser.Login();
                        }
                        knownKeyPressed = true;
                        break;

                    default: //Not known key pressed
                        Console.Clear();
                        Console.WriteLine("Wrong key, please try again.");
                        knownKeyPressed = false;
                        break;
                }
            } while (!knownKeyPressed);

            while (true)
            {
                BoardMenu(currentUser);

            }
        }

        public static void BoardMenu(User currentUser)
        {
            bool knownKeyPressed = false;
            do
            {
                Console.Clear();
                Console.WriteLine($"Logged in as {currentUser.Email}\n\nYour boards:");
                currentUser.ShowBoards();

                Console.WriteLine("\nPress 1 to create board, Press 2 to select board");
                ConsoleKeyInfo keyReaded = Console.ReadKey();
                Console.WriteLine();
                
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
                        SelectBoard(currentUser);
                        knownKeyPressed = true;
                        break;
                    case ConsoleKey.Q:
                        knownKeyPressed = false;
                        break;               
                }

            } while (!knownKeyPressed);
        }
        public static void SelectBoard(User current_user)
        {
            try
            {
                //permission check needed
                Console.WriteLine("Press id of board to select");
                int id_readed = Int32.Parse(Console.ReadLine());
                Board board = new Board(id_readed);
                Console.WriteLine($"board '{board.GetName()}' selected");
                Console.Clear();
                BoardEdit(board, current_user);
            }
            catch (Exception e)
            {
                Console.WriteLine("Enter correct id");
                Console.WriteLine(e);
                SelectBoard(current_user);
            }

        }
        public static void BoardEdit(Board board, User user)
        {
            bool knownKeyPressed = false;
            do
            {
                Console.Clear();
                Console.WriteLine($"Logged in as {user.Email}\n");

                Console.WriteLine($"Board: {board.GetName()}\n");
                board.ShowColumns();
                Console.WriteLine("\nPress 1 to change name of board, press 2 to add a column, press 3 to select column, press 4 to go back, press 5 to delete board");
                ConsoleKeyInfo consoleKey = Console.ReadKey();
                Console.WriteLine();
                switch (consoleKey.Key)
                {
                    case ConsoleKey.D1:
                        board.ChangeName();
                        knownKeyPressed = true;
                        break;
                    case ConsoleKey.D2:
                        board.AddColumn();
                        knownKeyPressed = true;
                        break;
                    case ConsoleKey.D5:
                        int user_id = board.GetUserId();
                        board.Delete();
                        knownKeyPressed = true;
                        BoardMenu(user);
                        break;
                    case ConsoleKey.D3:
                        board.SelectColumn();
                        knownKeyPressed = true;
                        break;
                    case ConsoleKey.D4:
                        int user_id2 = board.GetUserId();
                        knownKeyPressed = true;
                        User user2 = new User(user_id2);
                        BoardMenu(user2);
                        break;
                    default:
                        knownKeyPressed = false;
                        break;
                }
            }
            while (knownKeyPressed);
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
