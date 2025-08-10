using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Abstractions.DTOs
{
    public class ErpResponse<T>
    {
        public int TotalElements { get; set; }
        public int TotalPages { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public List<T> Content { get; set; } = new List<T>();
    }
}
