using Entegro.Application.Events;
using Entegro.Application.Interfaces.Event;
using Entegro.Application.Interfaces.Services.Marketplace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Services.Marketplace
{
    public class IdefixService : IIdefixService, IEventHandler<ProductIntegrationRecordUpdatedEvent>
    {
        public IdefixService() 
        {
        
        }

        public Task HandleAsync(ProductIntegrationRecordUpdatedEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
