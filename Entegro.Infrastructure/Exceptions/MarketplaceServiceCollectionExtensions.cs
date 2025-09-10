using Entegro.Application.Events;
using Entegro.Application.Interfaces;
using Entegro.Application.Interfaces.Services.Commerce;
using Entegro.Application.Interfaces.Services.Marketplace;
using Entegro.Application.Services.Commerce;
using Entegro.Application.Services.Commerce.Smartstore;
using Entegro.Application.Services.Marketplace;
using Entegro.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Entegro.Infrastructure.Exceptions
{
    public static class MarketplaceServiceCollectionExtensions
    {
        public static IServiceCollection AddMarketplaceServices(this IServiceCollection services)
        {
            services.AddScoped<ISmartstoreService, SmartstoreService>();
            services.AddScoped<ICommerceProductWriter, SmartstoreProductWriter>();
            services.AddScoped<IEventHandler<ProductIntegrationRecordUpdatedEvent>, SmartstoreProductWriter>();

            services.AddScoped<ITrendyolService, TrendyolService>();
            services.AddScoped<IEventHandler<ProductIntegrationRecordUpdatedEvent>, TrendyolService>();

            services.AddScoped<IN11Service, N11Service>();
            services.AddScoped<IEventHandler<ProductIntegrationRecordUpdatedEvent>, N11Service>();

            services.AddScoped<IEventPublisher, EventBus>();
            services.AddScoped<SmartstoreClient>();

            services.AddHttpClient();
            return services;
        }
    }
}
