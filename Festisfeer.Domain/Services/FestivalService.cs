using System.Collections.Generic;
using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;

namespace Festisfeer.Domain.Services
{
    public class FestivalService
    {
        private readonly IFestivalRepository _festivalRepository;

        // Constructor
        public FestivalService(IFestivalRepository festivalRepository)
        {
            _festivalRepository = festivalRepository;
        }

        // Alle festivals ophalen
        public List<Festival> GetFestivals()
        {
            return _festivalRepository.GetFestivals();
        }

        // Festival ophalen op basis van ID
        public Festival GetFestivalById(int id)
        {
            return _festivalRepository.GetFestivalById(id);
        }

        // Festival toevoegen
        public void AddFestival(Festival festival)
        {
            _festivalRepository.AddFestival(festival);
        }
    }
}