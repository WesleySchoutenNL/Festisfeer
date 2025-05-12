using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Festisfeer.Data.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly string _connectionString;

        public CommentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void AddComment(Comment comment)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var query = "INSERT INTO comments (Review_id, users_id, content, created_at) " +
                            "VALUES (@ReviewId, @UsersId, @Content, @CreatedAt)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ReviewId", comment.ReviewId);
                    cmd.Parameters.AddWithValue("@UsersId", comment.UserId);
                    cmd.Parameters.AddWithValue("@Content", comment.Content);
                    cmd.Parameters.AddWithValue("@CreatedAt", comment.CreatedAt);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Comment> GetCommentsByReviewId(int reviewId)
        {
            List<Comment> comments = new List<Comment>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT c.id, c.Review_id, c.users_id, c.content, c.created_at, u.username " +
                               "FROM comments c " +
                               "INNER JOIN users u ON u.id = c.users_id " +
                               "WHERE c.Review_id = @ReviewId " +
                               "ORDER BY c.created_at ASC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ReviewId", reviewId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comments.Add(new Comment
                            {
                                Id = reader.GetInt32("id"),
                                ReviewId = reader.GetInt32("Review_id"),
                                UserId = reader.GetInt32("users_id"),
                                Content = reader.GetString("content"),
                                CreatedAt = reader.GetDateTime("created_at"),
                                UserName = reader.GetString("username")
                            });
                        }
                    }
                }
            }

            return comments;
        }

        public Comment GetCommentById(int commentId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var query = "SELECT c.id, c.Review_id, c.users_id, c.content, c.created_at, u.username " +
                            "FROM comments c " +
                            "INNER JOIN users u ON u.id = c.users_id " +
                            "WHERE c.id = @CommentId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CommentId", commentId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Comment
                            {
                                Id = reader.GetInt32("id"),
                                ReviewId = reader.GetInt32("Review_id"),
                                UserId = reader.GetInt32("users_id"),
                                Content = reader.GetString("content"),
                                CreatedAt = reader.GetDateTime("created_at"),
                                UserName = reader.GetString("username")
                            };
                        }
                    }
                }
            }

            return null;
        }

        public void UpdateComment(Comment comment)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var query = "UPDATE comments SET content = @Content WHERE id = @Id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Content", comment.Content);
                    cmd.Parameters.AddWithValue("@Id", comment.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteComment(int commentId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var query = "DELETE FROM comments WHERE id = @Id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", commentId);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}