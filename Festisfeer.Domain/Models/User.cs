using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Festisfeer.Domain.Models
{
    public class User
    {
        public int Id { get; private set; }
        public string? Email { get; private set; }
        public string? Password { get; private set; }
        public string? Username { get; private set; }
        public string? Role { get; private set; }

        public User(int id, string email, string password, string username, string role) 
        {
            Id = id;
            Email = email;
            Password = password;
            Username = username;
            Role = role;
        }
    }

}