using Hyperledger.Aries.Max.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hyperledger.Aries.Max.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : TabbedPage, IRootView
	{
		public MainPage ()
		{
			InitializeComponent ();
		}

        private new void CurrentPageChanged(object sender, System.EventArgs e) => Title = GetPageName(CurrentPage);

        private new void Appearing(object sender, System.EventArgs e) => Title = GetPageName(CurrentPage);

        private string GetPageName(Page page)
        {
            if (page.BindingContext is ABaseViewModel vmBase)
                return vmBase.Name;
            return null;
        }
    }
}

