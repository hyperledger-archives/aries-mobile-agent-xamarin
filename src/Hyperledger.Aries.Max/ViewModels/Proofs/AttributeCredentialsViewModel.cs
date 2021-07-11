using Acr.UserDialogs;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Max.Extensions;
using Hyperledger.Aries.Max.Services.Interfaces;
using ReactiveUI;

namespace Hyperledger.Aries.Max.ViewModels.Proofs
{
    public class AttributeCredentialsViewModel : ABaseViewModel
    {
        private readonly IUserDialogs userDialogs;
        private readonly INavigationService navigationService;
        private bool _selected;
        string _credentialName;
        string _credentialConnection = string.Empty;

        public Credential Credential { get; }
        public string CredentialName
        {
            get => _credentialName;
            set
            {
                this.RaiseAndSetIfChanged(ref _credentialName, value);
            }
        }

        public string CredentialConnection
        {
            get => _credentialConnection;
            set
            {
                this.RaiseAndSetIfChanged(ref _credentialConnection, value);
            }
        }
        public string AttributeName { get; set; }

        public bool Selected
        {
            get => _selected;
            set
            {
                this.RaiseAndSetIfChanged(ref _selected, value);
            }
        }

        public string Referent { get; }

        public AttributeCredentialsViewModel(IUserDialogs userDialogs,
                                             INavigationService navigationService,
                                             Credential credential,
                                             string attributeName,
                                             string referent
                                            ) : base(nameof(AttributeCredentialsViewModel), userDialogs, navigationService)
        {
            this.userDialogs = userDialogs;
            this.navigationService = navigationService;
            Credential = credential;
            AttributeName = attributeName;
            Referent = referent;
            CredentialName = Credential.CredentialInfo.SchemaId.ToCredentialName();
        }
    }
}
