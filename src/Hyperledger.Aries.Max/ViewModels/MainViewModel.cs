using System.Threading.Tasks;
using Acr.UserDialogs;
using Hyperledger.Aries.Max.Services.Interfaces;
using Hyperledger.Aries.Max.ViewModels.Account;
using Hyperledger.Aries.Max.ViewModels.Connections;
using Hyperledger.Aries.Max.ViewModels.CreateInvitation;
using Hyperledger.Aries.Max.ViewModels.Credentials;
using Hyperledger.Aries.Max.ViewModels.Proofs;
using ReactiveUI;

namespace Hyperledger.Aries.Max.ViewModels
{
    public class MainViewModel : ABaseViewModel
    {
        public MainViewModel(
            IUserDialogs userDialogs,
            INavigationService navigationService,
            ConnectionsViewModel connectionsViewModel,
            CredentialsViewModel credentialsViewModel,
            AccountViewModel accountViewModel,
            CreateInvitationViewModel createInvitationViewModel,
            ProofRequestsViewModel proofRequestsViewModel) : base(nameof(MainViewModel), userDialogs, navigationService)
        {
            Connections = connectionsViewModel;
            Credentials = credentialsViewModel;
            Account = accountViewModel;
            CreateInvitation = createInvitationViewModel;
            ProofRequests = proofRequestsViewModel;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            await Connections.InitializeAsync(null);
            await Credentials.InitializeAsync(null);
            await Account.InitializeAsync(null);
            await CreateInvitation.InitializeAsync(null);
            await ProofRequests.InitializeAsync(null);
            await base.InitializeAsync(navigationData);
        }

        #region Bindable Properties

        private ConnectionsViewModel _connections;
        public ConnectionsViewModel Connections
        {
            get => _connections;
            set => this.RaiseAndSetIfChanged(ref _connections, value);
        }

        private CredentialsViewModel _credentials;
        public CredentialsViewModel Credentials
        {
            get => _credentials;
            set => this.RaiseAndSetIfChanged(ref _credentials, value);
        }

        private AccountViewModel _account;
        public AccountViewModel Account
        {
            get => _account;
            set => this.RaiseAndSetIfChanged(ref _account, value);
        }

        private CreateInvitationViewModel _createInvitation;
        public CreateInvitationViewModel CreateInvitation
        {
            get => _createInvitation;
            set => this.RaiseAndSetIfChanged(ref _createInvitation, value);
        }

        private ProofRequestsViewModel _proofRequests;
        public ProofRequestsViewModel ProofRequests
        {
            get => _proofRequests;
            set => this.RaiseAndSetIfChanged(ref _proofRequests, value);
        }

        #endregion
    }
}
