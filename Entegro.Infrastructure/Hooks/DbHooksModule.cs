using Autofac;
using Entegro.Application.Interfaces.Services;
using Entegro.Bootstrapping;
using Entegro.Data.Hooks;
using Entegro.Domain;
using Entegro.Domain.Entities.Catalog;
using Entegro.Engine;
using Entegro.Events;
using Entegro.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Infrastructure.Hooks
{
    public class DbHooksModule : Autofac.Module
    {
        private readonly IApplicationContext _appContext;

        public DbHooksModule(IApplicationContext appContext)
        {
            _appContext = appContext;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DefaultDbHookRegistry>().As<IDbHookRegistry>().SingleInstance();
            builder.RegisterType<DefaultDbHookActivator>().As<IDbHookActivator>().InstancePerDependency();
            builder.RegisterType<DefaultDbHookProcessor>().As<IDbHookProcessor>().InstancePerDependency();

            var hookTypes = _appContext.TypeScanner.FindTypes<IDbSaveHook>();
            foreach (var hookType in hookTypes)
            {
                var importantAttribute = hookType.GetAttribute<ImportantAttribute>(false);

                var types = DiscoverHookTypes(hookType);

                var serviceTypes = hookType.GetTypeInfo().ImplementedInterfaces
                   .Where(x => !x.IsGenericType)
                   .Except(EventsModule.IgnoredInterfaces.Concat(new[] { typeof(IDbSaveHook), typeof(IConsumer) }))
                   .ToArray();

                var registration = builder.RegisterType(hookType)
                    .As<IDbSaveHook>()
                    .InstancePerAttributedLifetime(fallback: ServiceLifetime.Transient)
                    .WithMetadata<HookMetadata>(m =>
                    {
                        m.For(em => em.HookedType, types.EntityType);
                        m.For(em => em.ServiceTypes, serviceTypes.Length > 0 ? serviceTypes : new[] { hookType });
                        m.For(em => em.ImplType, hookType);
                        m.For(em => em.DbContextType, types.ContextType ?? typeof(EntegroContext));
                        m.For(em => em.Importance, importantAttribute?.Importance ?? HookImportance.Normal);
                        m.For(em => em.Order, 0);
                    });

                if (serviceTypes.Length > 0)
                {
                    // This call actually overrides any former registration for the interface.
                    registration.As(serviceTypes);
                }
                else
                {
                    registration.AsSelf();
                }

            }
        }

        internal static (Type ContextType, Type EntityType) DiscoverHookTypes(Type type)
        {
            var x = type.BaseType;
            while (x != null && x != typeof(object))
            {
                if (x.IsGenericType)
                {
                    var gtd = x.GetGenericTypeDefinition();
                    if (gtd == typeof(AsyncDbSaveHook<>) || gtd == typeof(DbSaveHook<>))
                    {
                        return (typeof(EntegroContext), x.GetGenericArguments()[0]);
                    }
                    if (gtd == typeof(AsyncDbSaveHook<,>) || gtd == typeof(DbSaveHook<,>))
                    {
                        var args = x.GetGenericArguments();
                        return (args[0], args[1]);
                    }
                }

                x = x.BaseType;
            }

            foreach (var intface in type.GetInterfaces())
            {
                if (intface.IsGenericType)
                {
                    var gtd = intface.GetGenericTypeDefinition();
                    if (gtd == typeof(IDbSaveHook<>))
                    {
                        return (intface.GetGenericArguments()[0], typeof(BaseEntity));
                    }
                }
            }

            return (typeof(EntegroContext), typeof(BaseEntity));
        }
    }
}
