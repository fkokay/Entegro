using Entegro.Application.Interfaces.Services.Erp;
using Entegro.Application.Services.Erp;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Infrastructure.Extensions
{
    public static class ErpServiceCollectionExtensions
    {
        public static IServiceCollection AddErpServices(this IServiceCollection services)
        {
            services.AddScoped<IErpService, ErpService>();

            return services;
        }
    }
}
