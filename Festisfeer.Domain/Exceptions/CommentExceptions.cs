using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Festisfeer.Domain.Exceptions
{
    public class CommentExceptions
    {
        public class CommentRepositoryException : Exception
        {
            public CommentRepositoryException(string message, Exception innerException) : base(message, innerException) { }
        }

        public class CommentServiceException : Exception
        {
            public CommentServiceException(string message, Exception innerException)
                : base(message, innerException) { }
        }

        public class InvalidCommentDataException : ArgumentException
        {
            public InvalidCommentDataException(string message)
                : base(message) { }
        }
    }
}
