using System;
using System.Collections.Generic;
using System.Linq;
using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;

namespace Festisfeer.Domain.Services
{
    // Deze service zorgt voor de bedrijfslogica rondom festivals
    public class FestivalService
    {
        private readonly IFestivalRepository _festivalRepository;

        // Constructor met injectie van de repository (data toegang)
        public FestivalService(IFestivalRepository festivalRepository)
        {
            _festivalRepository = festivalRepository;
        }

        // Haalt alle festivals op
         public List<Festival> GetFestivals()
    {
        try
        {
            return _festivalRepository.GetFestivals();
        }
        catch (Exception ex)
        {
            throw new Exception("Fout bij ophalen van festivals via de service.", ex);
        }
    }

        // Haalt één festival op via het ID
        public Festival GetFestivalById(int id)
        {
            try
            {
                var festival = _festivalRepository.GetFestivalById(id);
                if (festival == null)
                {
                    throw new KeyNotFoundException($"Geen festival gevonden met ID {id}.");
                }

                return festival;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fout bij ophalen van festival met ID {id} via de service.", ex);
            }
        }

        // Voegt een nieuw festival toe, met validatie en logica
        public void AddFestival(Festival festival)
        {
            //Validatie: Datum check
            if (festival.EndDateTime < festival.StartDateTime)
            {
                throw new ArgumentException("De einddatum mag niet eerder zijn dan de startdatum.");
            }

            //Validatie: Naam mag niet leeg
            if (string.IsNullOrWhiteSpace(festival.Name))
            {
                throw new ArgumentException("De naam van het festival mag niet leeg zijn.");
            }

            //Validatie: Locatie mag niet leeg
            if (string.IsNullOrWhiteSpace(festival.Location))
            {
                throw new ArgumentException("De locatie van het festival mag niet leeg zijn.");
            }

            //Validatie: Ticketprijs moet positief zijn
            if (festival.TicketPrice < 0)
            {
                throw new ArgumentException("De ticketprijs mag niet negatief zijn.");
            }

            //Check op dubbele festivals (zelfde naam + zelfde datum)
            var bestaandFestival = _festivalRepository.GetFestivals()
                .Any(f => f.Name == festival.Name &&
                          f.StartDateTime.Date == festival.StartDateTime.Date);
                
            if (bestaandFestival)
            {
                throw new InvalidOperationException("Een festival met dezelfde naam op die datum bestaat al.");
            }

            // Opslaan in de repository
            _festivalRepository.AddFestival(festival);
        }
    }
}