using Festisfeer.Domain.Exceptions; // <-- Dit toevoegen bovenaan
using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Festisfeer.Domain.Services
{
    public class FestivalService
    {
        private readonly IFestivalRepository _festivalRepository;

        public FestivalService(IFestivalRepository festivalRepository)
        {
            _festivalRepository = festivalRepository;
        }

        public List<Festival> GetFestivals()
        {
            try
            {
                return _festivalRepository.GetFestivals();
            }
            catch (FestivalRepositoryException ex)
            {
                throw new FestivalServiceException("Fout bij ophalen van festivals via de service.", ex);
            }
        }

        public Festival GetFestivalById(int id)
        {
            try
            {
                var festival = _festivalRepository.GetFestivalById(id);
                if (festival == null)
                {
                    throw new FestivalNotFoundException(id); 
                }

                return festival;
            }
            catch (FestivalRepositoryException ex)
            {
                // Voeg context toe en gooi opnieuw
                throw new FestivalServiceException($"Fout bij ophalen van festival met ID {id} via service.", ex);
            }
        }

        public void AddFestival(Festival festival)
        {
            if (festival.EndDateTime < festival.StartDateTime)
            {
                throw new InvalidFestivalDataException("De einddatum mag niet eerder zijn dan de startdatum.");
            }

            if (string.IsNullOrWhiteSpace(festival.Name))
            {
                throw new InvalidFestivalDataException("De naam van het festival mag niet leeg zijn.");
            }

            if (string.IsNullOrWhiteSpace(festival.Location))
            {
                throw new InvalidFestivalDataException("De locatie van het festival mag niet leeg zijn.");
            }

            if (festival.TicketPrice < 0)
            {
                throw new InvalidFestivalDataException("De ticketprijs mag niet negatief zijn.");
            }

            var bestaandFestival = _festivalRepository.GetFestivals()
                .Any(f => f.Name == festival.Name &&
                          f.StartDateTime.Date == festival.StartDateTime.Date);

            if (bestaandFestival)
            {
                throw new DuplicateFestivalException(); 
            }

            _festivalRepository.AddFestival(festival);
        }
    }
}