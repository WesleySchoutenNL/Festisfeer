using System.Collections.Generic;
using Festisfeer.Domain.Models;

namespace Festisfeer.Domain.Interfaces
{
    public interface IFestivalRepository
    {
        List<Festival> GetFestivals(); // Alle festivals ophalen
        Festival GetFestivalById(int id); // Een specifiek festival ophalen op basis van ID
        void AddFestival(Festival festival); // Festival toevoegen
    }
}