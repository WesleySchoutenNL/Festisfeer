using MySql.Data.MySqlClient;
using Festisfeer.Domain.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;

namespace Festisfeer.Data.Repositories
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //Registreer een nieuwe gebruiker
        public void RegisterUser(User user)
        {
            // Controleer of de gebruiker al bestaat op basis van het e-mailadres
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string checkQuery = "SELECT COUNT(*) FROM users WHERE email = @Email";

                using (MySqlCommand cmd = new MySqlCommand(checkQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count > 0)
                    {
                        // Er bestaat al een gebruiker met dit e-mailadres
                        throw new Exception("Dit e-mailadres is al geregistreerd.");
                    }
                }
            }

            // Als de gebruiker nog niet bestaat, voeg deze toe
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO users (email, password, username, role) VALUES (@Email, @Password, @Username, @Role)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    cmd.Parameters.AddWithValue("@Username", user.Username);
                    cmd.Parameters.AddWithValue("@Role", "visitor");

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Inlog gebruiker (controleer email en wachtwoord)
        public User LoginUser(string email, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM users WHERE email = @Email";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedPassword = reader.GetString("password");
                            if (BCrypt.Net.BCrypt.Verify(password, storedPassword))
                            {
                                return new User
                                {
                                    Id = reader.GetInt32("id"),
                                    Email = reader.GetString("email"),
                                    Username = reader.GetString("username"),
                                    Role = reader.GetString("role")
                                };
                            }
                        }
                    }
                }
            }
            return null; // Invalid login
        }
    }
}