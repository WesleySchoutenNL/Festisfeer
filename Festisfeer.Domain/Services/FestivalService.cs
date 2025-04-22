using System.Collections.Generic;
using Festisfeer.Domain.Interfaces; 
using Festisfeer.Domain.Models;    

namespace Festisfeer.Domain.Services
{
    // Deze service zorgt voor de businesslogica rondom festivals
    public class FestivalService
    {
        private readonly IFestivalRepository _festivalRepository;

        // constructor
        public FestivalService(IFestivalRepository festivalRepository)
        {
            _festivalRepository = festivalRepository;
        }

        // Haalt alle festivals op via de interfacerepository
        public List<Festival> GetFestivals()
        {
            return _festivalRepository.GetFestivals();
        }

        // Haalt één festival op op basis van ID
        public Festival GetFestivalById(int id)
        {
            return _festivalRepository.GetFestivalById(id);
        }

        // Voegt een nieuw festival toe aan de database
        public void AddFestival(Festival festival)
        {
            _festivalRepository.AddFestival(festival);
        }
    }
}