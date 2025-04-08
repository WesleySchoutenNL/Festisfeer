using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using BCrypt.Net;

namespace Festisfeer.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Methode om te controleren of een gebruiker al bestaat (op basis van email of gebruikersnaam)
        public bool UserExists(string email, string username)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var query = "SELECT COUNT(*) FROM users WHERE email = @Email OR username = @Username";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Username", username);

            long count = (long)cmd.ExecuteScalar();
            return count > 0;
        }

        // Methode om een nieuwe gebruiker te registreren
        public void RegisterUser(User user)
        {
            if (UserExists(user.Email, user.Username))
            {
                throw new Exception("Gebruiker met dit e-mailadres of gebruikersnaam bestaat al.");
            }

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            // Het wachtwoord wordt gehasht met BCrypt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

            var query = "INSERT INTO users (email, password, username, role) VALUES (@Email, @Password, @Username, @Role)";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Password", hashedPassword);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@Role", "Visitor"); // Standaard rol
            cmd.ExecuteNonQuery();
        }

        // Methode om een gebruiker in te loggen (controleert of het wachtwoord correct is)
        public User? LoginUser(string email, string password)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var query = "SELECT * FROM users WHERE email = @Email";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", email);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string hashedPassword = reader.GetString("password");

                // Controleer of het opgegeven wachtwoord overeenkomt met het gehashte wachtwoord
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

                if (isPasswordValid)
                {
                    return new User
                    {
                        Id = reader.GetInt32("id"),
                        Email = reader.GetString("email"),
                        Password = hashedPassword, // Je hoeft het wachtwoord niet op te slaan, dit is ter voorbeeld
                        Username = reader.GetString("username"),
                        Role = reader.GetString("role")
                    };
                }
            }

            return null;
        }

        // Methode om een gebruiker op te halen op basis van hun ID
        public User? GetUserById(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var query = "SELECT * FROM users WHERE id = @Id";
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
                    Username = reader.GetString("username"),
                    Role = reader.GetString("role")
                };
            }

            return null;
        }
    }
}