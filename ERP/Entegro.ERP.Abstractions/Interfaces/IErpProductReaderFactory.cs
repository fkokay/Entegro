using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Abstractions.Interfaces
{
    public interface IErpProductReaderFactory
    {
        IErpProductReader Create(string erpType,string connectionString);
    }
}
