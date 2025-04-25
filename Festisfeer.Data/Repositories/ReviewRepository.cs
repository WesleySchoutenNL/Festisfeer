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

        public ReviewRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void AddReview(Review review)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var query = "INSERT INTO review (festival_id, users_id, content, rating, created_at) " +
                            "VALUES (@festival_id, @users_id, @content, @rating, @created_at)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@festival_id", review.FestivalId);
                    cmd.Parameters.AddWithValue("@users_id", review.UserId);
                    cmd.Parameters.AddWithValue("@content", review.Content);
                    cmd.Parameters.AddWithValue("@rating", review.Rating);
                    cmd.Parameters.AddWithValue("@created_at", review.CreatedAt);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Review> GetReviewsByFestivalId(int festivalId)
        {
            List<Review> reviews = new List<Review>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT r.id, r.content, r.rating, r.created_at, r.festival_id, r.users_id, u.username " +
                               "FROM review r " +
                               "INNER JOIN users u ON r.users_id = u.id " + // Hier voegen we de gebruikersnaam toe
                               "WHERE r.festival_id = @festivalId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@festivalId", festivalId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reviews.Add(new Review
                            {
                                Id = reader.GetInt32("id"),
                                Content = reader.GetString("content"),
                                Rating = reader.GetInt32("rating"),
                                CreatedAt = reader.GetDateTime("created_at"),
                                FestivalId = reader.GetInt32("festival_id"),
                                UserId = reader.GetInt32("users_id"),
                                UserName = reader.GetString("username") // Hier slaan we de gebruikersnaam op
                            });
                        }
                    }
                }
            }

            return reviews;
        }
    }
}