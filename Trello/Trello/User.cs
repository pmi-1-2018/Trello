using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace Trello
{
    public class User
    {
        String firstName;
        String lastName;
        String email;
        String password;
        int id;
        int currentBoardId;

        public User()
        {
            firstName = null;
            lastName = null;
            email = null;
            password = null;
            id = 0;
        }

        public User(String firstName, String lastName, String email, String password, int id=0)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.email = email;
            this.password = password;
            this.id = id;
        }

        public User(int id)
        {
            SQLiteConnection conn;
            conn = Program.CreateConnection();
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT firstName, lastName, email, password FROM users WHERE id={id}";
            SQLiteDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                this.firstName = (string)reader[0];
                this.lastName = (string)reader[1];
                this.email = (string)reader[2];
                this.password = (string)reader[3];
                this.id = id;
            }
            reader.Close();


            
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
        public int CurrentBoardId
        {
            get { return currentBoardId; }
            set { currentBoardId = value; }
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

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsPasswordStrong(string password)
        {
            return Regex.IsMatch(password, @"^(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?!.*\s).*$");
        }

        public User AutoLogin()
        {
            Console.WriteLine("LOGGING INTO APP\n");
            User user = new User();
            string login = "volod2@com";
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
                    string pass = "12V0v@34";
                    
                    if ((pass == p_word) && (pass != null))
                    {
                        user = new User(f_name, l_name, mail, p_word, pk);
                        reader.Close();
                        Console.WriteLine($"you logged in as {user.email}");
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
        public Boolean CreateBoard()
        {
            SQLiteConnection sql_con = Program.CreateConnection();
            string name;
            Console.WriteLine("Enter name of board:");
            name = Console.ReadLine();
            SQLiteCommand insertSQL = new SQLiteCommand("INSERT INTO boards (name, user) VALUES (@name, @user)", sql_con);
            insertSQL.Parameters.Add("name", System.Data.DbType.String).Value = name;
            insertSQL.Parameters.Add("user", System.Data.DbType.String).Value = this.id;
            try
            {
                insertSQL.ExecuteNonQuery();
                Console.WriteLine("board is created");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void ShowBoards()
        {
            SQLiteConnection sql_con = Program.CreateConnection();
            SQLiteCommand cmd = sql_con.CreateCommand();
            cmd.CommandText = $"SELECT * FROM boards where user={this.id};";
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                IDataRecord record = (IDataReader)reader;
                Console.WriteLine(String.Format("id:{0} -> {1}", record[0], record[1]));
            }
        }

        public Boolean Register()
        {
            SQLiteConnection sql_con = Program.CreateConnection();
            string f_name, l_name, mail, pass, pass2;
            Console.Write("CREATING NEW USER\nyour email: ");
            mail = Console.ReadLine();
            while (!IsValidEmail(mail))
            {
                Console.WriteLine("try again. your email is not valid\nyour email: ");
                mail = Console.ReadLine();
            }
            // to check if user already exist
            SQLiteDataReader reader;
            SQLiteCommand cmd;
            cmd = sql_con.CreateCommand();
            cmd.CommandText = "SELECT * FROM users WHERE email='" + mail + "';";
            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                Console.WriteLine("there is already user with such email\n type 'yes' to register new user or any other key to login");
                string resp = Console.ReadLine();
                if (resp=="yes")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            Console.Write("your first name: ");
            f_name = Console.ReadLine();
            Console.Write("your last name: ");
            l_name = Console.ReadLine();
            bool flag2 = true;
            while (flag2)
            {


                Console.WriteLine("your password:");
                pass = null;
                pass2 = null;
                bool flag = false;
                do
                {
                    while (true)
                    {
                        var key = System.Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Enter)
                            break;
                        pass += key.KeyChar;
                    }
                    if (flag)
                    {
                        Console.WriteLine("your password is not strong enough. it must be 8 characters and have both letters and numbers \ntry again");
                    }
                    flag = true;
                }
                while (!IsPasswordStrong(pass));

                Console.WriteLine("type your password again:");
                while (true)
                {
                    var key = System.Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                        break;
                    pass2 += key.KeyChar;
                }
                if (pass == pass2)
                {
                    User user = new User(f_name, l_name, mail, pass);
                    SQLiteCommand insertSQL = new SQLiteCommand("INSERT INTO users (firstName, lastName, email, password) VALUES (@firstName, @lastName, @email, @password)", sql_con);
                    insertSQL.Parameters.Add("firstName", System.Data.DbType.String).Value = user.firstName;
                    insertSQL.Parameters.Add("lastName", System.Data.DbType.String).Value = user.lastName;
                    insertSQL.Parameters.Add("email", System.Data.DbType.String).Value = user.email;
                    insertSQL.Parameters.Add("password", System.Data.DbType.String).Value = user.password;
                    try
                    {
                        insertSQL.ExecuteNonQuery();
                        Console.WriteLine("user added");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                Console.WriteLine("passwords do not match. try again.");
            }
            return true;
        }

    }
}