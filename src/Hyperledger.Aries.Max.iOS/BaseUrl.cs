using System;
using Foundation;
using Hyperledger.Aries.Max;
using Hyperledger.Aries.Max.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(BaseUrl))]
namespace Hyperledger.Aries.Max.iOS
{
    public class BaseUrl: IBaseUrl
    {
        public string Get()
        {
            return NSBundle.MainBundle.BundlePath;
        }
    }
}
