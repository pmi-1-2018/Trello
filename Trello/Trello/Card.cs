using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;

namespace Trello
{
    public class Card
    {
        int id;
        int columnId;
        string name;
        string description;
        List<string> tasks;
        SQLiteConnection conn;


        public Card(int id, int columnId)
        {
            this.id = id;
            this.columnId = columnId;
            conn = Program.CreateConnection();
            FetchDbData(id);
        }

        public Card()
        {
            this.id = 0;
            this.columnId = 0;
            this.name = null;
            this.description = null;
            this.tasks = null;
        }

        public Card(int id, int columnId, string name, string description) //List<string> tasks
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.columnId = columnId;
            this.tasks = new List<string>();
            //foreach (string task in tasks)
            //{
            //    this.tasks.Add(task);
            //}
            conn = Program.CreateConnection();
        }

        public void FetchDbData(int card_id)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT name, description FROM cards WHERE card_id={card_id}";
            SQLiteDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                this.name = (string)reader[0];
                this.description = (string)reader[1];
            }
            reader.Close();
        }

        public override string ToString()
        {
            string result = $"Card: {name}\nDESCRIPTION:\n{description}\n";
            return result;
        }

        public void ChangeName()
        {
            Console.WriteLine("Enter new card name:");
            string card_name = Console.ReadLine();
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"UPDATE cards SET card_name='{card_name}' WHERE card_id={id};";
            cmd.ExecuteNonQuery();
            Console.WriteLine(cmd.CommandText);

            name = card_name;
            Console.WriteLine($"Card name has been changed - {name}");
        }

        public void EditDescription()
        {
            Console.WriteLine($"Old description:\n{description}\nType new description:");
            string new_desc = Console.ReadLine();
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"UPDATE cards SET description='{new_desc}' WHERE card_id={id};";
            cmd.ExecuteNonQuery();
            Console.WriteLine(cmd.CommandText);

            description = new_desc;
            Console.WriteLine("Card descriptionhas been changed");
        }

        public void Move(int board_id)
        {
            Console.WriteLine($"Moving card {name} to another column");
            //get list of columns_id
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT id, name FROM columns WHERE board={board_id};";
            SQLiteDataReader reader = cmd.ExecuteReader();
            Console.WriteLine("Columns of this board: ");
            List<int> columns_id = new List<int>();
            while (reader.Read())
            {
                columns_id.Add((int)(long)reader[0]);
                Console.WriteLine($"id:{reader[0]} -> {reader[1]}");
            }
            reader.Close();

            Console.WriteLine("type id of column where you want to move this card");
            int new_id = Convert.ToInt32(Console.ReadLine());

            //move card to another column
            SQLiteCommand cmd2 = conn.CreateCommand();
            cmd2.CommandText = $"UPDATE cards SET column_id='{new_id}' WHERE card_id={id};";
            cmd2.ExecuteNonQuery();
            Console.WriteLine(cmd2.CommandText);

            columnId = new_id;
            Console.WriteLine($"Card has been moved to column #{new_id}");
        }

        public void Delete()
        {
            //add deleting cards
            Console.Write("are you sure you want to delete this card?\ntype 'yes' or any other key to discard: ");
            string key = Console.ReadLine();
            key = key.Trim();
            key = key.ToLower();
            if (key == "yes")
            {
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"DELETE FROM cards WHERE card_id={id}";
                cmd.ExecuteNonQuery();
            }
        }

        public string GetName()
        {
            return name;
        }
    }
}
