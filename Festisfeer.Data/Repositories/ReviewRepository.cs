using MySql.Data.MySqlClient;
using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Festisfeer.Domain.Exceptions;
using static Festisfeer.Domain.Exceptions.ReviewExceptions;

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
            try
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
            catch (MySqlException ex)
            {
                throw new FestivalRepositoryException($"Databasefout bij toevoegen van review '{review.Content}': {ex.Message}", ex);
            }

        }

        public List<Review> GetReviewsByFestivalId(int festivalId)
        {
            List<Review> reviews = new List<Review>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = "SELECT r.id, r.content, r.rating, r.created_at, r.festival_id, r.users_id, u.username " +
                                   "FROM review r " +
                                   "INNER JOIN users u ON r.users_id = u.id " +
                                   "WHERE r.festival_id = @festivalId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@festivalId", festivalId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Gebruik constructor in plaats van object initializer
                                var review = new Review(
                                    id: reader.GetInt32("id"),
                                    content: reader.GetString("content"),
                                    rating: reader.GetInt32("rating"),
                                    createdAt: reader.GetDateTime("created_at"),
                                    festivalId: reader.GetInt32("festival_id"),
                                    userId: reader.GetInt32("users_id"),
                                    userName: reader.GetString("username")
                                );

                                reviews.Add(review);
                            }
                        }
                    }
                }

                return reviews;
            }
            catch (MySqlException ex)
            {

                throw new ReviewRepositoryException($"Databasefout bij ophalen van reviews: {ex.Message}", ex);
            }
        }
    }
}