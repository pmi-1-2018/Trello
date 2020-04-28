using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;


namespace Trello
{
    public class Column
    {
        private string name;
        private int id;
        private int boardId;
        SQLiteConnection conn;
        //private int currentCardId;

        public Column(int id, int boardId)
        {
            this.id = id;
            this.boardId = boardId;
            conn = Program.CreateConnection();
            FetchDbData(id);
        }

        public Column()
        {
            this.id = 0;
            this.boardId = 0;
            this.name = null;
        }

        public Column(int id, string name, int boardId)
        {
            this.id = id;
            this.name = name;
            this.boardId = boardId;
            conn = Program.CreateConnection();
        }

        public void FetchDbData(int column_id)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT name FROM columns WHERE id={column_id} AND board={boardId}";
            SQLiteDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                this.name = (string)reader[0];
            }
            reader.Close();
        }

        public void ChangeName()
        {
            Console.WriteLine("Enter new column name:");
            string column_name = Console.ReadLine();
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"UPDATE columns SET name='{column_name}' WHERE id={id};";
            cmd.ExecuteNonQuery();
            Console.WriteLine(cmd.CommandText);

            name = column_name;
            Console.WriteLine($"Column name has been changed - {name}");
        }

        public string GetName()
        {
            return name;
        }

        public void AddCard()
        {
            Console.WriteLine("Enter new card name:");
            string card_name = Console.ReadLine();
            Console.WriteLine("Enter card description:");
            string description = Console.ReadLine();
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"INSERT INTO cards (column_id, card_name, description) VALUES ({id}, '{card_name}', '{description}');";
            cmd.ExecuteNonQuery();
            Console.WriteLine($"Card {card_name} is created");
        }
        public void DeleteCard()
        {
            Console.WriteLine("Enter card id to delete:");
            long card_id = long.Parse(Console.ReadLine());
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"DELETE FROM cards WHERE card_id={card_id};";
            cmd.ExecuteNonQuery();
            Console.WriteLine($"Card is deleted");
        }
        public void ShowCards()
        {
            Console.WriteLine("Column: " + name);
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT card_id, card_name, description FROM cards WHERE column_id={id};";
            SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"id:{reader[0]} -> name:{reader[1]}\n{reader[2]}");
            }
        }
    }
}
