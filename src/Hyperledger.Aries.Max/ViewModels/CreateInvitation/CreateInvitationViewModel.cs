using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Max.Services.Interfaces;
using ReactiveUI;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace Hyperledger.Aries.Max.ViewModels.CreateInvitation
{
    public class CreateInvitationViewModel : ABaseViewModel
    {
        private readonly IAgentProvider _agentContextProvider;
        private readonly IConnectionService _connectionService;

        public CreateInvitationViewModel(
            IUserDialogs userDialogs,
            INavigationService navigationService,
            IAgentProvider agentContextProvider,
            IConnectionService defaultConnectionService
            ) : base(
                "CreateInvitation",
                userDialogs,
                navigationService
           )
        {
            _agentContextProvider = agentContextProvider;
            _connectionService = defaultConnectionService;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            await base.InitializeAsync(navigationData);
        }

        private async Task CreateInvitation()
        {
            try
            {
                var context = await _agentContextProvider.GetContextAsync();
                var (invitation, _) = await _connectionService.CreateInvitationAsync(context, new InviteConfiguration
                {
                    TheirAlias = new ConnectionAlias { Name = "Invitation" }
                });

                string barcodeValue = invitation.ServiceEndpoint + "?d_m=" + Uri.EscapeDataString(invitation.ToByteArray().ToBase64String());
                QrCodeValue = barcodeValue;
            }
            catch (Exception ex)
            {
                DialogService.Alert(ex.Message);
            }
        }

        private ZXingBarcodeImageView QRCodeGenerator(string barcodeValue)
        {
            var barcode = new ZXingBarcodeImageView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                AutomationId = "zxingBarcodeImageView",
            };

            barcode.BarcodeFormat = ZXing.BarcodeFormat.QR_CODE;
            barcode.BarcodeOptions.Width = 300;
            barcode.BarcodeOptions.Height = 300;
            barcode.BarcodeOptions.Margin = 10;
            barcode.BarcodeValue = barcodeValue;

            return barcode;

        }

        #region Bindable Command

        public ICommand CreateInvitationCommand => new Command(async () => await CreateInvitation());

        #endregion

        #region Bindable Properties

        private string _qrCodeValue;

        public string QrCodeValue
        {
            get => _qrCodeValue;
            set => this.RaiseAndSetIfChanged(ref _qrCodeValue, value);
        }

        #endregion
    }
}
