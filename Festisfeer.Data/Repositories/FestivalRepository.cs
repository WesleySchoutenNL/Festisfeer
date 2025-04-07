using MySql.Data.MySqlClient;
using Festisfeer.Domain.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Festisfeer.Domain.Interfaces;

namespace Festisfeer.Data.Repositories
{
    public class FestivalRepository : IFestivalRepository
    {
        private readonly string _connectionString;

        public FestivalRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Haalt alle festivals op uit de database
        public List<Festival> GetFestivals()
        {
            List<Festival> festivals = new List<Festival>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM festival";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            festivals.Add(new Festival
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.GetString("name"),
                                Location = reader.GetString("location"),
                                StartDateTime = reader.GetDateTime("start_datetime"),
                                EndDateTime = reader.GetDateTime("end_datetime"),
                                Genre = reader.GetString("genre"),
                                TicketPrice = reader.GetInt32("ticket_price"),
                                FestivalImg = reader.GetString("image_url")
                            });
                        }
                    }
                }
            }

            return festivals;
        }

        // Haalt een specifiek festival op via ID
        public Festival GetFestivalById(int id)
        {
            Festival festival = null;

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM festival WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            festival = new Festival
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.GetString("name"),
                                Location = reader.GetString("location"),
                                StartDateTime = reader.GetDateTime("start_datetime"),
                                EndDateTime = reader.GetDateTime("end_datetime"),
                                Genre = reader.GetString("genre"),
                                TicketPrice = reader.GetInt32("ticket_price"),
                                FestivalImg = reader.GetString("image_url")
                            };
                        }
                    }
                }
            }

            return festival;
        }

        // Festival toevoegen aan de database
        public void AddFestival(Festival festival)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO festival (name, location, start_datetime, end_datetime, genre, ticket_price, image_url) " +
                               "VALUES (@name, @location, @start_datetime, @end_datetime, @genre, @ticket_price, @image_url)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", festival.Name);
                    cmd.Parameters.AddWithValue("@location", festival.Location);
                    cmd.Parameters.AddWithValue("@start_datetime", festival.StartDateTime);
                    cmd.Parameters.AddWithValue("@end_datetime", festival.EndDateTime);
                    cmd.Parameters.AddWithValue("@genre", festival.Genre);
                    cmd.Parameters.AddWithValue("@ticket_price", festival.TicketPrice);
                    cmd.Parameters.AddWithValue("@image_url", festival.FestivalImg);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}