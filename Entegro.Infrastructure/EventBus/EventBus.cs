using Entegro.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Infrastructure.EventBus
{
    public class EventBus : IEventPublisher
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EventBus(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void Publish<TEvent>(TEvent @event)
        {
            _ = Task.Run(async () =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;
                    var handlers = serviceProvider.GetServices<IEventHandler<TEvent>>();

                    var handlerTasks = handlers.Select(h => h.HandleAsync(@event)).ToList();
                    await Task.WhenAll(handlerTasks);
                }
            });
        }
    }
}
