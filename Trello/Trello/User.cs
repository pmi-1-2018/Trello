using System;
using System.Collections.Generic;
using System.Text;

namespace Trello
{
    class User
    {
        String firstName;
        String lastName;
        String email;
        String password;

        public User(String firstName, String lastName, String email, String password)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.email = email;
            this.password = password;
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

    }
}
