using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RaceDayAPI.Models;

namespace RaceDayAPI.Repositories
{
    public interface IUserRepository
    {
        User Add(User user);
        User? GetByEmail(string email);
    }

    public class AdoUserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public AdoUserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("RaceDay") ??
                throw new InvalidOperationException("Connection string 'RaceDay' is not configured.");
        }

        public User Add(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"INSERT INTO Users (Name, Email, Password, Role)
                                          VALUES (@Name, @Email, @Password, @Role);
                                          SELECT SCOPE_IDENTITY();";

                command.Parameters.AddWithValue("@Name", user.Name);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@Role", user.Role);

                connection.Open();
                var result = command.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                    throw new InvalidOperationException("Failed to retrieve inserted Id from database.");

                // SCOPE_IDENTITY() returns numeric/decimal, so handle accordingly
                if (result is decimal dec)
                    user.Id = Convert.ToInt32(dec);
                else
                    user.Id = Convert.ToInt32(result);
                return user;
            }
        }

        public User? GetByEmail(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Id, Name, Email, Password, Role FROM Users WHERE Email = @Email";
                command.Parameters.AddWithValue("@Email", email);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Email = reader.GetString(2),
                            Password = reader.GetString(3),
                            Role = reader.GetString(4)
                        };
                    }
                }

                return null;
            }
        }
    }
}
