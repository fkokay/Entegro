using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Service
{
    public class WorkerApplicationBuilder : IApplicationBuilder
    {
        public IServiceProvider ApplicationServices { get; set; }
        public IFeatureCollection ServerFeatures => new FeatureCollection();
        public IDictionary<string, object?> Properties { get; } = new Dictionary<string, object?>();

        public WorkerApplicationBuilder(IServiceProvider serviceProvider)
        {
            ApplicationServices = serviceProvider;
        }

        public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware) => this;
        public IApplicationBuilder New() => this;

        public RequestDelegate Build()
        {
            return context =>
            {
                return Task.CompletedTask;
            };
        }
    }
}
