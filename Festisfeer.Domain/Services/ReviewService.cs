using Festisfeer.Domain.Exceptions; // Voeg dit toe voor custom exceptions
using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;
using System;
using System.Collections.Generic;
using static Festisfeer.Domain.Exceptions.ReviewExceptions;

namespace Festisfeer.Domain.Services
{
    public class ReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public void AddReview(Review review)
        {

            if  (string.IsNullOrWhiteSpace(review.Content))
            {
                throw new InvalidReviewDataException("De review die je wil invullen is leeg");
            }

            if (review.Rating < 0)
            {
                throw new InvalidReviewDataException("De rating mag niet negatief zijn.");
            }

            try
            {
                _reviewRepository.AddReview(review);
            }
            catch (ReviewRepositoryException ex)
            {
                throw new ReviewServiceException("Fout bij toevoegen van review via de service.", ex);
            }
        }

        public List<Review> GetReviewsByFestivalId(int festivalId)
        {
            try
            {
                return _reviewRepository.GetReviewsByFestivalId(festivalId);
            }
            catch (ReviewRepositoryException ex)
            {
                throw new ReviewServiceException($"Fout bij ophalen van reviews voor festival ID {festivalId}.", ex);
            }
        }
    }
}