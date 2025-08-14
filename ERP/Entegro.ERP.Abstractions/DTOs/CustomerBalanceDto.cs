using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Abstractions.DTOs
{
    public class CustomerBalanceDto
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public DateTime LastTransactionDate { get; set; }
    }
}
