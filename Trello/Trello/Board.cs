using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace Trello
{
    public class Board
    {
        private int id;
        SQLiteConnection conn;
        private string name;
        private int user_id;
        User user;
        
        //public Board(int id, int user_id)
        //{
        //    this.user_id = user_id;
        //    this.id = id;
        //    user = new User(user_id);
        //    conn = Program.CreateConnection();
        //    FetchDbData(id);
        //}

        public Board(int id)
        {
            this.id = id;
            conn = Program.CreateConnection();
            FetchDbData(id);
        }
        public void FetchDbData(int board_id)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT name, user FROM boards WHERE id={board_id}";
            SQLiteDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                this.name = (string)reader[0];
                this.user_id = (int)(long)reader[1];
                user = new User(user_id);
            }
            reader.Close();

        }

        //public void FetchDbData(int board_id)
        //{
        //    SQLiteCommand cmd = conn.CreateCommand();
        //    cmd.CommandText = $"SELECT name FROM boards WHERE id={board_id} AND user={user_id}";
        //    SQLiteDataReader reader = cmd.ExecuteReader();

        //    if(reader.HasRows)
        //    {
        //        reader.Read();
        //        this.name = (string)reader[0];
        //    }
        //    reader.Close();

        //}

        public int GetUserId()
        {
            return user_id;
        }

        public void ChangeName()
        {
            Console.WriteLine("Enter new board name:");
            string board_name = Console.ReadLine();
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"UPDATE boards SET name='{board_name}' WHERE id={this.id};";
            Console.WriteLine(cmd.CommandText);
            cmd.ExecuteNonQuery();
            Console.WriteLine($"Board name is changed {board_name}");
            this.name = board_name;
        }

        public void Delete()
        {
            Console.Write("are you sure you want to delete this board?\ntype 'yes' or any other key to discard: ");
            string key = Console.ReadLine();
            key = key.Trim();
            key = key.ToLower();
            if (key == "yes")
            {
                List<int> columns_id = new List<int>();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT id FROM columns WHERE board={id};";
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    columns_id.Add((int)(long)reader[0]);
                }
                reader.Close();
                Console.WriteLine(columns_id);
                foreach (int elem in columns_id)
                {
                    cmd.CommandText = $"DELETE FROM cards where column_id={elem}";
                    cmd.ExecuteNonQuery();
                }

                cmd.CommandText = $"DELETE FROM columns where board={id}";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $"DELETE FROM boards WHERE id={id};";
                cmd.ExecuteNonQuery();
                Console.WriteLine($"Board {name} has been deleted!");
            }
        }

        public string GetName()
        {
            return name;
        }

        public User GetUser()
        {
            return user;
        }
        public void AddColumn()
        {
            Console.WriteLine("Enter new column name:");
            string column_name = Console.ReadLine();
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"INSERT INTO columns (name, board) VALUES ('{column_name}', {id});";
            cmd.ExecuteNonQuery();
            Console.WriteLine($"Column {column_name} is created");
        }
        
        public void ShowColumns()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT id,name FROM columns WHERE board={id};";
            SQLiteDataReader reader = cmd.ExecuteReader();
            Console.WriteLine("Columns: ");
            bool isEmpty = true;
            while (reader.Read())
            {
                isEmpty = false;
                Console.WriteLine($"id:{reader[0]} -> {reader[1]}");
            }
            if (isEmpty)
                Console.WriteLine("This board does not have any columns yet!");
            
            reader.Close();
        }
        

        public void SelectColumn()
        {
            try
            {
                //permission check needed
//                ShowColumns();
                Console.WriteLine("Press id of column to select:");
                int id_readed = Int32.Parse(Console.ReadLine());
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT * FROM columns WHERE id={id_readed};";
                SQLiteDataReader reader = cmd.ExecuteReader();
                reader.Read();
                
                Column column = new Column((int)(long)reader[0], (string)reader[1], (int)(long)reader[2]);
//                Console.WriteLine($"Column '{column.GetName()}' selected");
                reader.Close();
                Console.Clear();
                ColumnEdit(column);
            }
            catch (Exception e)
            {
                Console.WriteLine("Enter correct id");
                Console.WriteLine(e);
                SelectColumn();
            }
        }

        public void ColumnEdit(Column column)
        {
            bool knownKeyPressed = false;
            do
            {
                Console.Clear();
                Console.WriteLine($"Logged in as {user.Email}\n");
                Console.WriteLine($"Board: {this.name}\n");
                Console.WriteLine($"Column: {column.GetName()}\n");
                column.ShowCards();
                Console.WriteLine("\nPress 1 to change name, press 2 to add card, press 3 to select card, press 4 to go back, press 5 to delete column");
                ConsoleKeyInfo consoleKey = Console.ReadKey();
                Console.WriteLine();
                switch (consoleKey.Key)
                {
                    case ConsoleKey.D1:
                        column.ChangeName();
                        knownKeyPressed = true;
                        break;
                    case ConsoleKey.D2:
                        column.AddCard();
                        knownKeyPressed = true;
                        break;
                    case ConsoleKey.D3:
                        //check if column has cards
                        column.SelectCard();
                        knownKeyPressed = true;
                        break;
                    case ConsoleKey.D4:
                        knownKeyPressed = true;
                        Program.BoardEdit(this, user);
                        break;
                    case ConsoleKey.D5:
                        column.Delete();
                        knownKeyPressed = true;
                        //to add checking if board has columns
                        Program.BoardEdit(this, user);
                        break;
                    default:
                        knownKeyPressed = false;
                        break;
                }
            }
            while (knownKeyPressed);
        }


    }
}
