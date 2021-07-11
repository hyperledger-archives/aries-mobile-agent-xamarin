using Autofac;
using Hyperledger.Aries.Max;
using Hyperledger.Aries.Max.Services;

namespace Hyperledger.Aries.Max.iOS
{
    public class PlatformModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new CoreModule());
        }
    }
}