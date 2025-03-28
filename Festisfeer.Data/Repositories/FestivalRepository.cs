using MySql.Data.MySqlClient;
using Festisfeer.Domain.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Festisfeer.Data.Repositories
{
    public class FestivalRepository
    {
        private readonly string _connectionString;

        public FestivalRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // ✅ Haal alle festivals op
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
                                StartDate = reader.GetDateTime("start_date"),
                                EndDate = reader.GetDateTime("end_date"),
                                Genre = reader.GetString("genre"),
                                TicketPrice = reader.GetInt32("ticket_price"),
                                FestivalImg = reader.GetString("festival_img")
                            });
                        }
                    }
                }
            }

            return festivals;
        }

        // ✅ Voeg een nieuw festival toe
        public void AddFestival(Festival festival)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO festival (name, location, start_date, end_date, genre, ticket_price, festival_img) " +
                               "VALUES (@name, @location, @start_date, @end_date, @genre, @ticket_price, @festival_img)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", festival.Name);
                    cmd.Parameters.AddWithValue("@location", festival.Location);
                    cmd.Parameters.AddWithValue("@start_date", festival.StartDate);
                    cmd.Parameters.AddWithValue("@end_date", festival.EndDate);
                    cmd.Parameters.AddWithValue("@genre", festival.Genre);
                    cmd.Parameters.AddWithValue("@ticket_price", festival.TicketPrice);
                    cmd.Parameters.AddWithValue("@festival_img", festival.FestivalImg);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}