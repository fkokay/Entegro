using Entegro.Application.Interfaces.Event;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Infrastructure.Messaging
{
    public class EventBus : IEventPublisher
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EventBus(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task Publish<TEvent>(TEvent @event)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var handlers = serviceProvider.GetServices<IEventHandler<TEvent>>();

                foreach (var handler in handlers)
                {
                    await handler.HandleAsync(@event);
                }
            }
        }
    }
}
