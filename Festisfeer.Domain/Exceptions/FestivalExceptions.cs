using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Festisfeer.Domain.Exceptions
{
    public class FestivalNotFoundException : Exception
    {
        public FestivalNotFoundException(int id)
            : base($"Geen festival gevonden met ID {id}.") { }
    }

    public class DuplicateFestivalException : Exception
    {
        public DuplicateFestivalException()
            : base("Een festival met dezelfde naam op die datum bestaat al.") { }
    }

    public class InvalidFestivalDataException : ArgumentException
    {
        public InvalidFestivalDataException(string message)
            : base(message) { }
    }
    public class FestivalServiceException : Exception
    {
        public FestivalServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
    public class FestivalRepositoryException : Exception
    {
        public FestivalRepositoryException(string message, Exception innerException) : base(message, innerException) { }
    }
}
