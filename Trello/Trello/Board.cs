using System;
using System.Data;
using System.Data.SQLite;
namespace Trello
{
    public class Board
    {
        private long id;
        SQLiteConnection conn;
        private string name;
        private long user_id;
        
        public Board(int id, long user_id)
        {
            this.user_id = user_id;
            this.id = id;
            Console.WriteLine($"USER_ID {user_id}");
            conn = Program.CreateConnection();
            FetchDbData(id);

        }
        public void FetchDbData(long board_id)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT name FROM boards WHERE id={board_id} AND user={user_id}";
            SQLiteDataReader reader = cmd.ExecuteReader();

            if(reader.HasRows)
            {
                reader.Read();
                this.name = (string)reader[0];
            }

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
            long column_id = long.Parse(Console.ReadLine());
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
                Console.WriteLine($"id:{reader[0]}, name:{reader[1]}");
            }
        }
        public void DeleteColumn(string column_id)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"DELETE FROM columns where id={column_id};";
            Console.WriteLine(cmd.CommandText);
            cmd.ExecuteNonQuery();
            Console.WriteLine("Column is deleted");
        }
    }
}
