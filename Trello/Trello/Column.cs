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
        Board board;
        SQLiteConnection conn;
        
        public Column(int id, string name, int boardId)
        {
            this.id = id;
            this.name = name;
            this.boardId = boardId;
            this.board = new Board(boardId);
            conn = Program.CreateConnection();
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
            Console.WriteLine("\nEnter new card name:");
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
                Console.Clear();
                Console.WriteLine($"Logged in as {board.GetUser().Email}\n");
                Console.WriteLine($"Board: {board.GetName()}\n");
                Console.WriteLine(card);
                Console.WriteLine("\nPress 1 to change name, press 2 to edit description, press 3 to move card to another column, press 4 to go back, press 5 to delete card");
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
                        board.ColumnEdit(this);
                        break;
                    case ConsoleKey.D4:
                        knownKeyPressed = true;
                        board.ColumnEdit(this);
                        break;
                    case ConsoleKey.D5:
                        card.Delete();
                        knownKeyPressed = true;
                        //to add checking if column has cards
                        board.ColumnEdit(this);
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
            Console.Write("\nare you sure you want to delete this column?\ntype 'yes' or any other key to discard: ");
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
            Console.WriteLine("\nEnter card id to delete:");
            long card_id = long.Parse(Console.ReadLine());
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"DELETE FROM cards WHERE card_id={card_id};";
            cmd.ExecuteNonQuery();
            Console.WriteLine($"Card is deleted");
        }
        public void ShowCards()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT card_id, card_name FROM cards WHERE column_id={id};";
            SQLiteDataReader reader = cmd.ExecuteReader();
            Console.WriteLine("Cards: ");
            bool isEmpty = true;
            while (reader.Read())
            {
                isEmpty = false;
                Console.WriteLine($"id:{reader[0]} -> name: {reader[1]}");
            }
            if (isEmpty)
                Console.WriteLine("There is no cards in this column!");
        }
    }
}
