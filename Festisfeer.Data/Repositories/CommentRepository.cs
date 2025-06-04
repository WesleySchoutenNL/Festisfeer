using Festisfeer.Domain.Exceptions;
using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using static Festisfeer.Domain.Exceptions.CommentExceptions;

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
            try
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
            catch (MySqlException ex)
            {
                throw new CommentRepositoryException($"Databasefout bij toevoegen van festival '{comment.Content}': {ex.Message}", ex);
            }

        }

        public List<Comment> GetCommentsByReviewId(int reviewId)
        {
            List<Comment> comments = new List<Comment>();

            try
            {
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
                                var comment = new Comment(
                                    id: reader.GetInt32("id"),
                                    reviewId: reader.GetInt32("Review_id"),
                                    userId: reader.GetInt32("users_id"),
                                    content: reader.GetString("content"),
                                    createdAt: reader.GetDateTime("created_at"),
                                    userName: reader.GetString("username")
                                );

                                comments.Add(comment);

                            }
                        }
                    }
                }

                return comments;
            }
            catch (MySqlException ex)
            {
                throw new CommentRepositoryException($"Databasefout bij toevoegen van festival {ex.Message}", ex);
            }

        }

        public Comment GetCommentById(int commentId)
        {
            try
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
                                var comment = new Comment(
                                    id: reader.GetInt32("id"),
                                    reviewId: reader.GetInt32("Review_id"),
                                    userId: reader.GetInt32("users_id"),
                                    content: reader.GetString("content"),
                                    createdAt: reader.GetDateTime("created_at"),
                                    userName: reader.GetString("username")
                                );

                                return comment;
                            }
                        }
                    }
                }

                return null;
            }
            catch (MySqlException ex)
            {
                throw new CommentRepositoryException($"Databasefout bij ophalen van comment met ID {commentId}: {ex.Message}", ex);
            }
        }

        public void UpdateComment(Comment comment)
        {
            try
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
            catch (MySqlException ex)
            {
                throw new CommentRepositoryException($"Databasefout bij updaten van comment met ID {comment.Id}: {ex.Message}", ex);
            }
        }

        public void DeleteComment(int commentId)
        {
            try
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
            catch (MySqlException ex)
            {
                throw new CommentRepositoryException($"Databasefout bij verwijderen van comment met ID {commentId}: {ex.Message}", ex);
            }
        }
    }
}