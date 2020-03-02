using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;

namespace Trello
{
    class User
    {
        String firstName;
        String lastName;
        String email;
        String password;
        int id;

        public User()
        {
            firstName = null;
            lastName = null;
            email = null;
            password = null;
            id = 0;
        }

        public User(String firstName, String lastName, String email, String password, int id)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.email = email;
            this.password = password;
            this.id = id;
        }

        public string FirstName   // property
        {
            get { return firstName; }   // get method
            set { firstName = value; }  // set method
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public override string ToString()
        {
            return firstName + " " + lastName; 
        }

        public User Login()
        {
            Console.WriteLine("LOGGING INTO APP\ntype your email:");
            User user = new User();
            string login = Console.ReadLine();
            SQLiteConnection sqliteConn;
            sqliteConn = Program.CreateConnection();

            SQLiteDataReader reader;
            SQLiteCommand cmd;
            cmd = sqliteConn.CreateCommand();
            cmd.CommandText = "SELECT * FROM users WHERE email='" + login + "';";
            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string f_name = reader.GetString(0);
                    string l_name = reader.GetString(1);
                    string mail = reader.GetString(2);
                    string p_word = reader.GetString(3);
                    int pk = reader.GetInt32(4);
                    Console.WriteLine("type your password:");
                    string pass = null;
                    while (true)
                    {
                        var key = System.Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Enter)
                            break;
                        pass += key.KeyChar;
                    }
                    if ((pass == p_word) && (pass != null))
                    {
                        user = new User(f_name, l_name, mail, p_word, pk);
                        reader.Close();
                        Console.WriteLine("you logged in!");
                        return user;
                    }
                    else
                    {
                        Console.WriteLine("you entered wrong password.\n");
                        reader.Close();
                        return user;
                    }
                }
                reader.Close();
                return user;
            }
            else
            {
                Console.WriteLine("No such user found.\n");
                reader.Close();
                return user;
            }           
        }
    }
}