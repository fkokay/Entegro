using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.N11
{
    public class N11ApiContext
    {
        public string BaseUrl = " https://api.n11.com/";
        public string AppKey { get; set; }
        public string AppSecret { get; set; }
    }
}
