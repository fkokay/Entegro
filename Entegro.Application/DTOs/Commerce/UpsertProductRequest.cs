using Entegro.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Commerce
{
    public class UpsertProductRequest
    {
        public ProductDto Product { get; set; }
        public object CustomData { get; set; }
    }
}
