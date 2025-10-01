using Entegro.Application.Events;
using Entegro.Application.Interfaces.Event;
using Entegro.Application.Interfaces.Services.Commerce;
using Entegro.Application.Interfaces.Services.Erp;
using Entegro.Application.Services.Commerce;
using Entegro.Application.Services.Commerce.Smartstore;
using Entegro.Application.Services.Erp;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Infrastructure.Extensions
{
    public static class CommerceServiceCollectionExtensions
    {
        public static IServiceCollection AddCommerceServices(this IServiceCollection services)
        {
            services.AddScoped<ISmartstoreService, SmartstoreService>();
            services.AddScoped<ICommerceProductWriter, SmartstoreProductWriter>();
            services.AddScoped<IEventHandler<ProductIntegrationRecordUpdatedEvent>, SmartstoreProductWriter>();

            services.AddScoped<SmartstoreClient>();
            return services;
        }
    }
}
