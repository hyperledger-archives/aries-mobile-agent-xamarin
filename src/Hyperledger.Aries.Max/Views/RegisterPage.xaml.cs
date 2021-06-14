using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hyperledger.Aries.Max.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegisterPage : ContentPage, IRootView
    {
		public RegisterPage ()
		{
			InitializeComponent ();
		}
	}
}