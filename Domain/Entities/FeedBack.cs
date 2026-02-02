using RestaurantOrderTracking.Domain.Common;
using System;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class FeedBack : BaseEntities
    {
        public Guid OrderId { get; private set; }
        public virtual Order Order { get; private set; } = null!;

        public int Rating { get; private set; }
        public string? Comment { get; private set; }
        public bool IsAnonymous { get; private set; }

        protected FeedBack() { }

        public FeedBack(Guid orderId, int rating, string? comment = null, bool isAnonymous = false)
        {
            if (rating < 1 || rating > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");
            }

            OrderId = orderId;
            Rating = rating;
            Comment = comment;
            IsAnonymous = isAnonymous;
        }

        public void UpdateFeedback(int rating, string? comment)
        {
            if (rating < 1 || rating > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");
            }

            Rating = rating;
            Comment = comment;
        }
    }
}
