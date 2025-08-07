using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Common
{
    public interface INamedEntity
    {
        string GetEntityName();
    }

    public interface IDisplayedEntity : INamedEntity
    {
        string[] GetDisplayNameMemberNames();
        string GetDisplayName();
    }
}
