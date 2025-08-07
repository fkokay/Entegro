using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Exceptions
{
    public abstract class DomainException : Exception
    {
        protected DomainException() { }

        protected DomainException(string message) : base(message) { }

        protected DomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}
