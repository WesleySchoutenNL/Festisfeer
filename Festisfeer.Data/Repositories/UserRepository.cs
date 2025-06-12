using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using BCrypt.Net;
using static Festisfeer.Domain.Exceptions.AccountExceptions;

namespace Festisfeer.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public bool UserExists(string email, string username)
        {
            try
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
            catch (MySqlException ex)
            {
                throw new AccountRepositoryException("Fout bij controleren of gebruiker bestaat.", ex);
            }
        }

        public void RegisterUser(User user)
        {
            try
            {
                if (UserExists(user.Email, user.Username))
                {
                    throw new AccountRepositoryException("Gebruiker met dit e-mailadres of gebruikersnaam bestaat al.");
                }

                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                var query = "INSERT INTO users (email, password, username, role) VALUES (@Email, @Password, @Username, @Role)";
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Password", hashedPassword);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@Role", "Visitor"); // Standaard rol
                cmd.ExecuteNonQuery();
            }
            catch (AccountRepositoryException)
            {
                throw; // Hergooi repository exception zoals het is
            }
            catch (MySqlException ex)
            {
                throw new AccountRepositoryException("Fout bij registreren gebruiker in database.", ex);
            }
        }

        public User? LoginUser(string email, string password)
        {
            try
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

                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                    if (isPasswordValid)
                    {
                        return new User(
                            reader.GetInt32("id"),
                            reader.GetString("email"),
                            hashedPassword,
                            reader.GetString("username"),
                            reader.GetString("role")
                        );
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new AccountRepositoryException("Fout bij inloggen van gebruiker in database.", ex);
            }
        }

        public User? GetUserById(int id)
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                var query = "SELECT * FROM users WHERE id = @Id";
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new User(
                        reader.GetInt32("id"),
                        reader.GetString("email"),
                        reader.GetString("password"),
                        reader.GetString("username"),
                        reader.GetString("role")
                    );
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new AccountRepositoryException("Fout bij ophalen gebruiker uit database.", ex);
            }
        }
    }
}