using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Commerce.Smartstore
{
    public class SmartstoreProductAttributeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Alias { get; set; }
        public bool AllowFiltering { get; set; }
        public int DisplayOrder { get; set; }
        public string FacetTemplateHint { get; set; }
        public bool IndexOptionNames { get; set; }
        public string? ExportMappings { get; set; }
    }
}
