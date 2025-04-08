using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Festisfeer.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void RegisterUser(User user)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var query = "INSERT INTO user (email, password, username) VALUES (@Email, @Password, @Username)";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Password", user.Password); // In de praktijk zou dit gehashed moeten zijn
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.ExecuteNonQuery();
        }

        public User? LoginUser(string email, string password)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var query = "SELECT * FROM user WHERE email = @Email AND password = @Password";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", password);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt32("id"),
                    Email = reader.GetString("email"),
                    Password = reader.GetString("password"),
                    Username = reader.GetString("username")
                };
            }

            return null;
        }

        public User? GetUserById(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var query = "SELECT * FROM user WHERE id = @Id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt32("id"),
                    Email = reader.GetString("email"),
                    Password = reader.GetString("password"),
                    Username = reader.GetString("username")
                };
            }

            return null;
        }
    }
}