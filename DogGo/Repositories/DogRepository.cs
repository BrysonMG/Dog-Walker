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
                }
            }
        }

        void IDogRepository.DeleteDog(int dogId)
        {
            throw new NotImplementedException();
        }

        List<Dog> IDogRepository.GetAllDogs()
        {
            throw new NotImplementedException();
        }

        Dog IDogRepository.GetDogById(int id)
        {
            throw new NotImplementedException();
        }

        void IDogRepository.UpdateDog(Dog dog)
        {
            throw new NotImplementedException();
        }
    }
}
