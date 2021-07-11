using Acr.UserDialogs;
using Autofac;
using Hyperledger.Aries.Max.Services;
using Microsoft.Extensions.Logging;

namespace Hyperledger.Aries.Max
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            
            builder
                .Register(x => UserDialogs.Instance)
                .As<IUserDialogs>()
                .SingleInstance();

            builder
                .RegisterType<NavigationService>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder
                .RegisterType<KeyValueStoreService>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder
                .Register(_ => new LoggerFactory())
                .As<ILoggerFactory>()
                .SingleInstance();

            builder
                .RegisterGeneric(typeof(Logger<>))
                .As(typeof(ILogger<>))
                .SingleInstance();

            builder
                .RegisterAssemblyTypes(ThisAssembly)
                .Where(x => x.Namespace.Contains("Hyperledger.Aries.Max.ViewModels"))
                .InstancePerDependency();

            builder
                .RegisterAssemblyTypes(ThisAssembly)
                .Where(x => x.Namespace.Contains("Hyperledger.Aries.Max.Views"))
                .InstancePerDependency();
        }
    }
}
