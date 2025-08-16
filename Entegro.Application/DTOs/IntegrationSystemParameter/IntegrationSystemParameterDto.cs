using Entegro.Application.DTOs.IntegrationSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.IntegrationSystemParameter
{
    public class IntegrationSystemParameterDto
    {
        public int Id { get; set; }
        public int IntegrationSystemId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public virtual IntegrationSystemDto IntegrationSystem { get; set; }
    }
}
