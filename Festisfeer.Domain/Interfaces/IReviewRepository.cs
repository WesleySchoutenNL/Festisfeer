using System.Collections.Generic;
using Festisfeer.Domain.Models;

namespace Festisfeer.Domain.Interfaces
{
    public interface IReviewRepository
    {
        void AddReview(Review review);
        List<Review> GetReviewsByFestivalId(int festivalId);
    }
}
