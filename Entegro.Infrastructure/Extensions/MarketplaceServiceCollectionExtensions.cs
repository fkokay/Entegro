using Entegro.Application.Events;
using Entegro.Application.Interfaces;
using Entegro.Application.Interfaces.Services.Commerce;
using Entegro.Application.Interfaces.Services.Marketplace;
using Entegro.Application.Services.Commerce;
using Entegro.Application.Services.Commerce.Smartstore;
using Entegro.Application.Services.Marketplace;
using Entegro.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Entegro.Infrastructure.Extensions
{
    public static class MarketplaceServiceCollectionExtensions
    {
        public static IServiceCollection AddMarketplaceServices(this IServiceCollection services)
        {
            services.AddScoped<ITrendyolService, TrendyolService>();
            services.AddScoped<IEventHandler<ProductIntegrationRecordUpdatedEvent>, TrendyolService>();

            services.AddScoped<IN11Service, N11Service>();
            services.AddScoped<IEventHandler<ProductIntegrationRecordUpdatedEvent>, N11Service>();

            services.AddScoped<IPazaramaService, PazaramaService>();
            services.AddScoped<IEventHandler<ProductIntegrationRecordUpdatedEvent>, PazaramaService>();

            services.AddScoped<IHepsiburadaService, HepsiburadaService>();
            services.AddScoped<IEventHandler<ProductIntegrationRecordUpdatedEvent>, HepsiburadaService>();

            services.AddScoped<ICicekSepetiService, CicekSepetiService>();
            services.AddScoped<IEventHandler<ProductIntegrationRecordUpdatedEvent>, CicekSepetiService>();
            return services;
        }
    }
}
