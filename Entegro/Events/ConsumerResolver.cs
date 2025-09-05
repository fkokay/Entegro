using System.Reflection;
using Autofac;
using Entegro.Engine;
using Entegro.Engine.Modularity;

namespace Entegro.Events
{
    public class ConsumerResolver : IConsumerResolver
    {
        public virtual IConsumer Resolve(ConsumerDescriptor descriptor)
        {
            var scope = EngineContext.Current.Scope;

            if (descriptor.ModuleDescriptor == null || scope.Resolve<IModuleConstraint>().Matches(descriptor.ModuleDescriptor, null))
            {
                return scope.ResolveKeyed<IConsumer>(descriptor.ContainerType);
            }

            return null;
        }

        public virtual object ResolveParameter(ParameterInfo p, IComponentContext c = null)
        {
            return (c ?? EngineContext.Current.Scope.RequestContainer).Resolve(p.ParameterType);
        }
    }
}