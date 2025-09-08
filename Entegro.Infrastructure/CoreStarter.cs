using Autofac;
using Entegro.Engine;
using Entegro.Engine.Builders;
using Entegro.Infrastructure.Hooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Infrastructure
{
    internal class CoreStarter : StarterBase
    {
        public override int Order => (int)StarterOrdering.Early;

        public override void ConfigureContainer(ContainerBuilder builder, IApplicationContext appContext)
        {
            builder.RegisterModule(new DbHooksModule(appContext));
        }
    }
}
