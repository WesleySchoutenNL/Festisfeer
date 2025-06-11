using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Festisfeer.Domain.Exceptions
{
    public class ReviewExceptions : Exception
    {
        public class ReviewRepositoryException : Exception
        {
            public ReviewRepositoryException(string message, Exception innerException) : base(message, innerException) { }
        }

        public class ReviewServiceException : Exception
        {
            public ReviewServiceException(string message, Exception innerException)
                : base(message, innerException) { }
        }

        public class InvalidReviewDataException : ArgumentException
        {
            public InvalidReviewDataException(string message)
                : base(message) { }
        }
    }
}
