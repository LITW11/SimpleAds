using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SimpleAds.Data
{
    public class SimpleAdDb
    {
        private readonly string _connectionString;

        public SimpleAdDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddSimpleAd(SimpleAd ad)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Ads (Description, Name, PhoneNumber, DateCreated) " +
                                  "VALUES (@desc, @name, @phone, GETDATE()) SELECT SCOPE_IDENTITY()";
            
            command.Parameters.AddWithValue("@desc", ad.Description);
            command.Parameters.AddWithValue("@name", ad.Name != null ? ad.Name : DBNull.Value);
            command.Parameters.AddWithValue("@phone", ad.PhoneNumber);
            connection.Open();
            ad.Id = (int)(decimal)command.ExecuteScalar();
        }

        public List<SimpleAd> GetAds()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Ads ORDER BY DateCreated DESC";
            connection.Open();
            List<SimpleAd> ads = new List<SimpleAd>();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(GetAdFromReader(reader));
            }

            return ads;
        }

        public SimpleAd GetById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Ads " +
                                  "WHERE a.Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return GetAdFromReader(reader);
        }

        public void Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Ads WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }

        private SimpleAd GetAdFromReader(SqlDataReader reader)
        {
            var ad = new SimpleAd
            {
                Name = reader.GetOrNull<string>("Name"),
                Description = reader.GetOrNull<string>("Description"),
                Date = reader.GetOrNull<DateTime>("DateCreated"),
                PhoneNumber = reader.GetOrNull<string>("PhoneNumber"),
                Id = reader.GetOrNull<int>("Id")
            };
            return ad;
        }
    }
}