using System;
using System.Data;
using System.Data.SQLite;
namespace Trello
{
    public class Board
    {
        private int id;
        SQLiteConnection conn;
        private string name;
        private int user_id;
//        private long currentColumn;
        
        public Board(int id, int user_id)
        {
            this.user_id = user_id;
            this.id = id;
            conn = Program.CreateConnection();
            FetchDbData(id);
        }

        public void FetchDbData(int board_id)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT name FROM boards WHERE id={board_id} AND user={user_id}";
            SQLiteDataReader reader = cmd.ExecuteReader();

            if(reader.HasRows)
            {
                reader.Read();
                this.name = (string)reader[0];
            }
            reader.Close();

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

        public string GetName()
        {
            return name;
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
        public void DeleteColumn()
        {
            Console.WriteLine("Enter column id to delete:");
            int column_id = int.Parse(Console.ReadLine());
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"DELETE FROM columns WHERE id={column_id};";
            cmd.ExecuteNonQuery();
            Console.WriteLine($"Column is deleted");
        }
        public void ShowColumns()
        {
            Console.WriteLine("Board: " + name);
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT id,name FROM columns WHERE board={id};";
            SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"id:{reader[0]} -> {reader[1]}");
            }
            reader.Close();
        }
        

        public void SelectColumn()
        {
            try
            {
                //permission check needed
                Console.WriteLine("Press id of column to select:");
                int id_readed = Int32.Parse(Console.ReadLine());
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT * FROM columns WHERE id={id_readed};";
                SQLiteDataReader reader = cmd.ExecuteReader();
                reader.Read();
                
                Column column = new Column((int)(long)reader[0], (string)reader[1], (int)(long)reader[2]);
                Console.WriteLine($"Column '{column.GetName()}' selected");
                reader.Close();
                ColumnEdit(column);
            }
            catch (Exception e)
            {
                Console.WriteLine("Enter correct id");
                Console.WriteLine(e);
                this.SelectColumn();
            }
        }

        public static void ColumnEdit(Column column)
        {
            bool knownKeyPressed = false;
            do
            {
                Console.WriteLine($"\nColumn: {column.GetName()}");
                Console.WriteLine("Press 1 to change name, press 2 to add card, press 3 to delete card by id, press 4 to show all cards, press 5 to select card");
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
                        column.DeleteCard();
                        knownKeyPressed = true;
                        break;
                    case ConsoleKey.D4:
                        column.ShowCards();
                        knownKeyPressed = true;
                        break;
                    //case ConsoleKey.D5:
                    //    board.SelectColumn();
                    //    knownKeyPressed = true;
                    //    break;
                    default:
                        knownKeyPressed = false;
                        break;
                }
            }
            while (knownKeyPressed);
        }


    }
}
