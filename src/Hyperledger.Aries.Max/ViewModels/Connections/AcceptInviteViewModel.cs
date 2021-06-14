using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Max.Events;
using Hyperledger.Aries.Max.Services.Interfaces;
using ReactiveUI;
using Xamarin.Forms;

namespace Hyperledger.Aries.Max.ViewModels.Connections
{
    public class AcceptInviteViewModel : ABaseViewModel
    {
        private readonly IProvisioningService _provisioningService;
        private readonly IConnectionService _connectionService;
        private readonly IMessageService _messageService;
        private readonly IAgentProvider _contextProvider;
        private readonly IEventAggregator _eventAggregator;

        private ConnectionInvitationMessage _invite;

        public AcceptInviteViewModel(IUserDialogs userDialogs,
                                     INavigationService navigationService,
                                     IProvisioningService provisioningService,
                                     IConnectionService connectionService,
                                     IMessageService messageService,
                                     IAgentProvider contextProvider,
                                     IEventAggregator eventAggregator)
                                     : base("Accept Invitiation", userDialogs, navigationService)
        {
            _provisioningService = provisioningService;
            _connectionService = connectionService;
            _contextProvider = contextProvider;
            _messageService = messageService;
            _contextProvider = contextProvider;
            _eventAggregator = eventAggregator;
        }

        public override Task InitializeAsync(object navigationData)
        {
            if (navigationData is ConnectionInvitationMessage invite)
            {
                InviteTitle = $"Trust {invite.Label}?";
                InviterUrl = invite.ImageUrl;
                InviteContents = $"{invite.Label} would like to establish a pairwise DID connection with you. This will allow secure communication between you and {invite.Label}.";
                _invite = invite;
            }
            return base.InitializeAsync(navigationData);
        }

        #region Bindable Commands
        public ICommand AcceptInviteCommand => new Command(async () =>
        {
            var loadingDialog = DialogService.Loading("Processing");
            var context = await _contextProvider.GetContextAsync();

            try
            {
                var (msg, rec) = await _connectionService.CreateRequestAsync(context, _invite);
                msg.Label = "AriesMax";
                await _messageService.SendAsync(context, msg, rec);

                _eventAggregator.Publish(new ApplicationEvent() { Type = ApplicationEventType.ConnectionsUpdated });
            }
            finally
            {
                loadingDialog.Hide();
                await NavigationService.PopModalAsync();
            }
        });

        public ICommand RejectInviteCommand => new Command(async () => await NavigationService.PopModalAsync());

        #endregion

        #region Bindable Properties
        private string _inviteTitle;
        public string InviteTitle
        {
            get => _inviteTitle;
            set => this.RaiseAndSetIfChanged(ref _inviteTitle, value);
        }

        private string _inviteContents = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua";
        public string InviteContents
        {
            get => _inviteContents;
            set => this.RaiseAndSetIfChanged(ref _inviteContents, value);
        }

        private string _inviterUrl;
        public string InviterUrl
        {
            get => _inviterUrl;
            set => this.RaiseAndSetIfChanged(ref _inviterUrl, value);
        }
        #endregion
    }
}
