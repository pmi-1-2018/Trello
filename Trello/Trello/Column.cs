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

        public void SelectCard()
        {
            try
            {
                //permission check needed
                ShowCards();
                Console.WriteLine("Type id of card to select:");
                int id_readed = Int32.Parse(Console.ReadLine());
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT * FROM cards WHERE card_id={id_readed};";
                SQLiteDataReader reader = cmd.ExecuteReader();
                reader.Read();

                Card card = new Card((int)(long)reader[0], (int)(long)reader[1], (string)reader[2], (string)reader[3]);
                Console.WriteLine($"Card '{card.GetName()}' selected");
                reader.Close();
                CardEdit(card);
            }
            catch (Exception e)
            {
                Console.WriteLine("Enter correct id");
                Console.WriteLine(e);
                SelectCard();
            }
        }

        public void CardEdit(Card card)
        {
            bool knownKeyPressed = false;
            do
            {
                Console.WriteLine(card);

                Console.WriteLine("Press 1 to change name, press 2 to edit description, press 3 to move card to another column, press 4 to delete card");
                ConsoleKeyInfo consoleKey = Console.ReadKey();
                Console.WriteLine();
                switch (consoleKey.Key)
                {
                    case ConsoleKey.D1:
                        card.ChangeName();
                        knownKeyPressed = true;
                        break;
                    case ConsoleKey.D2:
                        card.EditDescription();
                        knownKeyPressed = true;
                        break;
                    case ConsoleKey.D3:
                        card.Move(this.boardId);
                        //check if is empty
                        knownKeyPressed = true;
                        break;
                    case ConsoleKey.D4:
                        card.Delete();
                        knownKeyPressed = true;
                        //to add checking if column has cards
                        this.SelectCard();
                        break;
                    default:
                        knownKeyPressed = false;
                        break;
                }
            }
            while (knownKeyPressed);
        }

        public void Delete()
        {
            //add deleting cards
            Console.Write("are you sure you want to delete this column?\ntype 'yes' or any other key to discard: ");
            string key = Console.ReadLine();
            key = key.Trim();
            key = key.ToLower();
            if (key == "yes")
            {
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"DELETE FROM cards WHERE column_id={id}";
                cmd.ExecuteNonQuery();
                
                cmd.CommandText = $"DELETE FROM columns WHERE id={id};";
                cmd.ExecuteNonQuery();
                Console.WriteLine($"Column is deleted");
            }
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
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT card_id, card_name, description FROM cards WHERE column_id={id};";
            SQLiteDataReader reader = cmd.ExecuteReader();
            Console.WriteLine("Cards: ");
            bool isEmpty = true;
            while (reader.Read())
            {
                isEmpty = false;
                Console.WriteLine($"id:{reader[0]} -> name:{reader[1]}\n{reader[2]}");
            }
            if (isEmpty)
                Console.WriteLine("There is no cards in this column!");
        }
    }
}
