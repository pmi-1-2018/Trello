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
                Console.WriteLine("Press 1 to create board, Press 2 to see your boards, Press 3 to select board");
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
                        currentUser.ShowBoards();
                        knownKeyPressed = true;
<<<<<<< HEAD
                        break;
                    case ConsoleKey.D3:
                        SelectBoard(currentUser);
                        knownKeyPressed = true;
                        break;
                    case ConsoleKey.Q:
                        knownKeyPressed = false;
                        break;
=======
                        break;
                    case ConsoleKey.D3:
                        SelectBoard(currentUser);
                        knownKeyPressed = true;
                        break;
                    case ConsoleKey.Q:
                        knownKeyPressed = false;
                        break;
>>>>>>> 31b7827... added board selecting and chaning its name
                        
                }

            } while (!knownKeyPressed);
        }
        public static void SelectBoard(User current_user)
        {

                Console.WriteLine("Press id of board to select show board details");
                try
                {
                    int id_readed = Int32.Parse(Console.ReadLine());
                    Board board = new Board(id_readed, current_user.Id);
                    BoardEdit(board);
                    
                }
                catch(Exception e)
                {
                    Console.WriteLine("Enter correct id");
                    Console.WriteLine(e);
                    SelectBoard(current_user);
                }
            
        }
        public static void BoardEdit(Board board)
        {
<<<<<<< HEAD
            bool knownKeyPressed = true;
            do
            {
                Console.WriteLine("BoardEdit\nPress 1 to change name, press 2 to add column, press 3 to delete column by id, press 4 to show Board data");
                ConsoleKeyInfo consoleKey = Console.ReadKey();
                Console.WriteLine();
=======
            bool knownKeyPressed = false;
            do
            {
                Console.WriteLine("BoardEdit\nPress 1 to change name, press 2 to add column, press 3 to delete column by id, press 4 to show all columns");
                ConsoleKeyInfo consoleKey = Console.ReadKey();
>>>>>>> 31b7827... added board selecting and chaning its name
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
<<<<<<< HEAD
                    case ConsoleKey.D3:
                        board.DeleteColumn();
                        knownKeyPressed = true;
                        break;
                    case ConsoleKey.D4:
                        board.ShowColumns();
                        knownKeyPressed = true;
                        break;
=======
>>>>>>> 31b7827... added board selecting and chaning its name
                    default:
                        knownKeyPressed = false;
                        break;
                }
            }
<<<<<<< HEAD
            while (knownKeyPressed);
=======
            while (!knownKeyPressed);
>>>>>>> 31b7827... added board selecting and chaning its name
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
