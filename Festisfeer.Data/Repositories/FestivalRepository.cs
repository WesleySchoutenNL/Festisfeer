using MySql.Data.MySqlClient;
using Festisfeer.Domain.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Festisfeer.Domain.Interfaces;
using System;

namespace Festisfeer.Data.Repositories
{
    public class FestivalRepository : IFestivalRepository
    {
        private readonly string _connectionString;

        public FestivalRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Festival> GetFestivals()
        {
            List<Festival> festivals = new List<Festival>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    var query = "SELECT * FROM festival ORDER BY start_datetime ASC";

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
                                    Location = reader.GetString("lsocation"),
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
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Databasefout bij ophalen van festivals: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Onverwachte fout bij ophalen van festivals: {ex.Message}", ex);
            }

            return festivals;
        }

        public Festival GetFestivalById(int id)
        {
            Festival festival = null;

            try
            {
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
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Databasefout bij ophalen van festival met ID {id}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Onverwachte fout bij ophalen van festival met ID {id}: {ex.Message}", ex);
            }

            return festival;
        }

        public void AddFestival(Festival festival)
        {
            try
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
            catch (MySqlException ex)
            {
                throw new Exception($"Databasefout bij toevoegen van festival '{festival.Name}': {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Onverwachte fout bij toevoegen van festival '{festival.Name}': {ex.Message}", ex);
            }
        }
    }
}