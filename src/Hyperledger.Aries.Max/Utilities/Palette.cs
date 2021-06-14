using Xamarin.Forms;

namespace Hyperledger.Aries.Max.Utilities
{
    public struct Palette
    {
        public Color BasePageColor;

        public void Init()
        {
            var resources = Application.Current.Resources;
            BasePageColor = (Color)resources["BasePageColor"];
        }
    }
}
