using DogGo.Interfaces;
using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Repositories
{
    public class DogRepository : IDogRepository
    {
        private readonly IConfiguration _config;

        public DogRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        void IDogRepository.AddDog(Dog dog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Dog ([Name], OwnerId, Breed, Notes, ImageUrl)
                        OUTPUT INSERTED.ID
                        VALUES (@name, @ownerId, @breed, @notes, @imageUrl);";

                    cmd.Parameters.AddWithValue("@name", dog.Name);
                    cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);
                    cmd.Parameters.AddWithValue("@breed", dog.Breed);
                    cmd.Parameters.AddWithValue("@notes", dog.Notes);
                    cmd.Parameters.AddWithValue("@imageUrl", dog.ImageUrl);

                    int id = (int)cmd.ExecuteScalar();

                    dog.Id = id;
                }
            }
        }

        void IDogRepository.DeleteDog(int dogId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        DELETE FROM Dog
                        WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@id", dogId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        List<Dog> IDogRepository.GetAllDogs()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], OwnerId, Breed, Notes, ImageUrl
                        FROM Dog";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Dog> dogs = new List<Dog>();

                    while (reader.Read())
                    {
                        Dog dog = new Dog
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                            Breed = reader.GetString(reader.GetOrdinal("Breed")),
                            Notes = reader.GetString(reader.GetOrdinal("Notes")),
                            ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl"))
                        };

                        dogs.Add(dog);
                    }
                    reader.Close();

                    return dogs;
                }
            }
        }

        Dog IDogRepository.GetDogById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], OwnerId, Breed, Notes, ImageUrl
                        FROM Dog
                        WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Dog dog = new Dog
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                            Breed = reader.GetString(reader.GetOrdinal("Breed")),
                            Notes = reader.GetString(reader.GetOrdinal("Notes")),
                            ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl"))
                        };

                        reader.Close();
                        return dog;
                    }
                    else
                    {
                        reader.Close();
                        return null;
                    }
                }
            }
        }

        void IDogRepository.UpdateDog(Dog dog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE Dog
                        SET
                            [Name] = @name,
                            OwnerId = @ownerId,
                            Breed = @breed,
                            Notes = @notes,
                            ImageUrl = @imageUrl
                        WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@name", dog.Name);
                    cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);
                    cmd.Parameters.AddWithValue("@breed", dog.Breed);
                    cmd.Parameters.AddWithValue("@notes", dog.Notes);
                    cmd.Parameters.AddWithValue("@imageUrl", dog.ImageUrl);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Dog> GetDogsByOwnerId(int ownerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT Id, Name, Breed, Notes, ImageUrl, OwnerId 
                FROM Dog
                WHERE OwnerId = @ownerId
            ";

                    cmd.Parameters.AddWithValue("@ownerId", ownerId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Dog> dogs = new List<Dog>();

                    while (reader.Read())
                    {
                        Dog dog = new Dog()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Breed = reader.GetString(reader.GetOrdinal("Breed")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId"))
                        };

                        // Check if optional columns are null
                        if (reader.IsDBNull(reader.GetOrdinal("Notes")) == false)
                        {
                            dog.Notes = reader.GetString(reader.GetOrdinal("Notes"));
                        }
                        if (reader.IsDBNull(reader.GetOrdinal("ImageUrl")) == false)
                        {
                            dog.ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl"));
                        }

                        dogs.Add(dog);
                    }
                    reader.Close();
                    return dogs;
                }
            }
        }

    }
}
