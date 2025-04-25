using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

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
            _reviewRepository.AddReview(review);
        }

        public List<Review> GetReviewsByFestivalId(int festivalId)
        {
            return _reviewRepository.GetReviewsByFestivalId(festivalId);
        }

    }
}