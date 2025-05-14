using MySql.Data.MySqlClient;                
using Festisfeer.Domain.Interfaces;          
using Festisfeer.Domain.Models;              
using Microsoft.Extensions.Configuration;     
using System;
using System.Collections.Generic;

namespace Festisfeer.Data.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly string _connectionString;

        // Constructor die de connectiestring uit de configuratie ophaalt (appsettings.json bijvoorbeeld)
        public ReviewRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Voegt een nieuwe review toe aan de database
        public void AddReview(Review review)
        {
            // Maakt een nieuwe connectie aan met de database
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open(); // Open de connectie

                // SQL query om een review toe te voegen
                var query = "INSERT INTO review (festival_id, users_id, content, rating, created_at) " +
                            "VALUES (@festival_id, @users_id, @content, @rating, @created_at)";

                //SqlCommand aan om de query met parameters uit te voeren
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Voeg waarden toe aan de SQL-parameters
                    cmd.Parameters.AddWithValue("@festival_id", review.FestivalId);
                    cmd.Parameters.AddWithValue("@users_id", review.UserId);
                    cmd.Parameters.AddWithValue("@content", review.Content);
                    cmd.Parameters.AddWithValue("@rating", review.Rating);
                    cmd.Parameters.AddWithValue("@created_at", review.CreatedAt);

                    // Voer de insert uit
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Haalt alle reviews op van een bepaald festival (via festivalId)
        public List<Review> GetReviewsByFestivalId(int festivalId)
        {
            List<Review> reviews = new List<Review>(); // Lijst om alle gevonden reviews in op te slaan

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open(); // Open de connectie

                // SQL-query om reviews op te halen inclusief gebruikersnaam van wie de review schreef
                string query = "SELECT r.id, r.content, r.rating, r.created_at, r.festival_id, r.users_id, u.username " +
                               "FROM review r " +
                               "INNER JOIN users u ON r.users_id = u.id " + // Join met users om gebruikersnaam op te halen
                               "WHERE r.festival_id = @festivalId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Parameter instellen
                    cmd.Parameters.AddWithValue("@festivalId", festivalId);

                    // Uitvoeren van de query en uitlezen van resultaten
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) // Loop door alle gevonden resultaten
                        {
                            // Maak een nieuwe Review aan en vul die met data uit de reader
                            reviews.Add(new Review
                            {
                                Id = reader.GetInt32("id"),
                                Content = reader.GetString("content"),
                                Rating = reader.GetInt32("rating"),
                                CreatedAt = reader.GetDateTime("created_at"),
                                FestivalId = reader.GetInt32("festival_id"),
                                UserId = reader.GetInt32("users_id"),
                                UserName = reader.GetString("username") // Gebruikersnaam ophalen uit de join
                            });
                        }
                    }
                }
            }

            return reviews; // Retourneer de lijst met reviews
        }
    }
}