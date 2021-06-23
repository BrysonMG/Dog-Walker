using DogGo.Models;
using System.Collections.Generic;

namespace DogGo.Interfaces
{
    public interface INeighborhoodRepository
    {
        List<Neighborhood> GetAll();
    }
}
