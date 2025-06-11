using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Festisfeer.Domain.Exceptions
{
    public class AccountExceptions
    {

        public class AccountServiceException : Exception
        {
            public AccountServiceException(string message, Exception innerException)
                : base(message, innerException) { }
        }
    }
}
