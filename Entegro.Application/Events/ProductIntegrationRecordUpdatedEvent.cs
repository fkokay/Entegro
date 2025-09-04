using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Events
{
    public class ProductIntegrationRecordUpdatedEvent
    {
        public int ProductIntegrationId { get; }

        public ProductIntegrationRecordUpdatedEvent(int productIntegrationId)
        {
            ProductIntegrationId = productIntegrationId;
        }
    }
}
