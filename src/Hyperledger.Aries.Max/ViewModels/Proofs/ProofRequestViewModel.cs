using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Autofac;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Aries.Max.Events;
using Hyperledger.Aries.Max.Services.Interfaces;
using Newtonsoft.Json;
using ReactiveUI;
using Xamarin.Forms;

namespace Hyperledger.Aries.Max.ViewModels.Proofs
{
    public class ProofRequestViewModel : ABaseViewModel
    {
        private readonly IUserDialogs userDialogs;
        private readonly INavigationService navigationService;
        private readonly IAgentProvider agentContextProvider;
        private readonly IProofService proofService;
        private readonly ILifetimeScope scope;
        private readonly ICredentialService credentialService;
        private readonly IConnectionService connectionService;
        private readonly IEventAggregator eventAggregator;
        private readonly IMessageService messageService;
        private readonly ProofRecord proofRecord;
        private readonly RequestPresentationMessage requestPresentationMessage;
        private readonly ConnectionRecord connection;
        private ObservableCollection<ProofRequestAttributeViewModel> _requestedAttributes = new ObservableCollection<ProofRequestAttributeViewModel>();

        /// <summary>
        /// This constructor is used when proof request details are fetched from mediator
        /// </summary>        
        public ProofRequestViewModel(IUserDialogs userDialogs,
                                     INavigationService navigationService,
                                     IAgentProvider agentContextProvider,
                                     IProofService proofService,
                                     ILifetimeScope scope,
                                     ICredentialService credentialService,
                                     IConnectionService connectionService,
                                     IEventAggregator eventAggregator,
                                     IMessageService messageService,
                                     ProofRecord proofRecord,
                                     ConnectionRecord connection) : base("Proof Request Detail", userDialogs, navigationService)
        {
            this.userDialogs = userDialogs;
            this.navigationService = navigationService;
            this.agentContextProvider = agentContextProvider;
            this.proofService = proofService;
            this.scope = scope;
            this.credentialService = credentialService;
            this.connectionService = connectionService;
            this.eventAggregator = eventAggregator;
            this.messageService = messageService;
            this.proofRecord = proofRecord;
            this.connection = connection;
            ConnectionLogo = connection.Alias.ImageUrl;
            ConnectionName = connection.Alias.Name;
            ProofRequest = JsonConvert.DeserializeObject<ProofRequest>(proofRecord.RequestJson);
            ProofRequestName = ProofRequest?.Name;
            RequestedAttributes = new ObservableCollection<ProofRequestAttributeViewModel>();
            HasLogo = !string.IsNullOrWhiteSpace(ConnectionLogo);
        }

        /// <summary>
        /// This constructor is used presentation request is scanned
        /// </summary>        
        public ProofRequestViewModel(IUserDialogs userDialogs,
                                    INavigationService navigationService,
                                    IAgentProvider agentContextProvider,
                                    IProofService proofService,
                                    ILifetimeScope scope,
                                    ICredentialService credentialService,
                                    IConnectionService connectionService,
                                    IEventAggregator eventAggregator,
                                    IMessageService messageService,
                                    ProofRequest proofRequest,
                                    RequestPresentationMessage requestPresentationMessage) : base("Proof Request Detail", userDialogs, navigationService)
        {
            this.userDialogs = userDialogs;
            this.navigationService = navigationService;
            this.agentContextProvider = agentContextProvider;
            this.proofService = proofService;
            this.scope = scope;
            this.credentialService = credentialService;
            this.connectionService = connectionService;
            this.eventAggregator = eventAggregator;
            this.messageService = messageService;
            this.requestPresentationMessage = requestPresentationMessage;
            ConnectionName = "Scanned Presentation Request";
            ProofRequest = proofRequest;
            ProofRequestName = ProofRequest?.Name;
            RequestedAttributes = new ObservableCollection<ProofRequestAttributeViewModel>();
            HasLogo = !string.IsNullOrWhiteSpace(ConnectionLogo);
        }

        public override async Task InitializeAsync(object navigationData)
        {
            await GetRequestedAttributes();
            await base.InitializeAsync(navigationData);
        }

        /// <summary>
        /// Reference https://github.com/hyperledger/aries-framework-dotnet/blob/master/test/Hyperledger.Aries.Tests/Protocols/ProofTests.cs#L644
        /// </summary>
        public async Task GetRequestedAttributes()
        {
            try
            {
                RequestedAttributes.Clear();

                if (ProofRequest.RequestedAttributes == null)
                    return;

                var context = await agentContextProvider.GetContextAsync();

                //Get any Available Credentials for each requested attribute
                foreach (var requestedAttribute in ProofRequest.RequestedAttributes)
                {
                    List<Credential> attributeCredentials = await proofService.ListCredentialsForProofRequestAsync(context, ProofRequest, requestedAttribute.Key);

                    var attribute = scope.Resolve<ProofRequestAttributeViewModel>(
                        new NamedParameter("name", requestedAttribute.Value.Name ?? string.Join(", ", requestedAttribute.Value.Names)),
                        new NamedParameter("isPredicate", false),
                        new NamedParameter("attributeCredentials", attributeCredentials),
                        new NamedParameter("referent", requestedAttribute.Key)
                    );

                    RequestedAttributes.Add(attribute);
                }

                //TODO: Implement Predicate and Restrictions related functionlity
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Reference https://github.com/hyperledger/aries-framework-dotnet/blob/master/src/Hyperledger.Aries.TestHarness/AgentScenarios.cs#L214
        /// </summary>
        private async Task CreatePresentation()
        {
            base.IsBusy = true;

            try
            {
                var requestedCredentials = new RequestedCredentials();
                var requestedAttributes = RequestedAttributes.Where(x => x.SelectedCredential != null && !x.IsPredicate)
                                           .Select(y => new KeyValuePair<string, RequestedAttribute>(y.AttributeReferent, new RequestedAttribute
                                           {
                                               CredentialId = y.SelectedCredential.Credential.CredentialInfo.Referent,
                                               Revealed = true
                                           })
                                           ).ToDictionary(z => z.Key, z => z.Value);

                //TODO: Implement Predicate related functionlity

                if (requestedAttributes != null && requestedAttributes.Count > 0)
                {
                    requestedCredentials.RequestedAttributes = requestedAttributes;

                    var context = await agentContextProvider.GetContextAsync();
                    if (requestPresentationMessage != null)
                    {
                        var result = await proofService.CreatePresentationAsync(context, requestPresentationMessage, requestedCredentials);
                    }
                    else
                    {
                        var (proofMsg, holderRecord) = await proofService.CreatePresentationAsync(context, proofRecord.Id, requestedCredentials);
                        await messageService.SendAsync(context, proofMsg, connection);
                    }

                }
                eventAggregator.Publish(new ApplicationEvent() { Type = ApplicationEventType.RefreshProofRequests });
                await NavigationService.NavigateBackAsync();
            }

            catch (Exception exception)
            {
                await Application.Current.MainPage.DisplayAlert("Error", exception.Message, "Ok");
            }
            finally
            {
                base.IsBusy = false;
            }

        }


        /// <summary>
        /// Reference https://github.com/hyperledger/aries-framework-dotnet/blob/master/test/Hyperledger.Aries.Tests/Protocols/ProofTests.cs#L838
        /// </summary>
        async Task DismissProofRequest()
        {
            var isConfirmed = await Application.Current.MainPage.DisplayAlert("Confirm", "Are you sure you want to decline this request?", "Yes", "No");
            if (!isConfirmed)
                return;

            try
            {
                IsBusy = true;
                var agentContext = await agentContextProvider.GetContextAsync();
                string id = proofRecord?.Id;
                if (id != null)
                {
                    await proofService.RejectProofRequestAsync(agentContext, id);
                    eventAggregator.Publish(new ApplicationEvent() { Type = ApplicationEventType.RefreshProofRequests });
                }

                await NavigationService.NavigateBackAsync();
            }
            catch (Exception xx)
            {
                await Application.Current.MainPage.DisplayAlert("Error", xx.Message, "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }

        #region Commands

        public ICommand PresentProofCommand => new Command(async () => await CreatePresentation());
        public ICommand DismissProofRequestCommand => new Command(async () => await DismissProofRequest());

        public ICommand ViewAttributeDetailCommand => new Command<ProofRequestAttributeViewModel>(async (attribute) =>
        {
            if (attribute == null)
                return;

            await NavigationService.NavigateToAsync(attribute);
        });

        #endregion

        #region Properties


        string _connectionLogo;
        public string ConnectionLogo
        {
            get => _connectionLogo;
            set => this.RaiseAndSetIfChanged(ref _connectionLogo, value);
        }

        bool _hasLogo = false;
        public bool HasLogo
        {
            get => _hasLogo;
            set => this.RaiseAndSetIfChanged(ref _hasLogo, value);
        }

        string _connectionName;
        public string ConnectionName
        {
            get => _connectionName;
            set => this.RaiseAndSetIfChanged(ref _connectionName, value);
        }

        string _proofRequestName;
        public string ProofRequestName
        {
            get => _proofRequestName;
            set => this.RaiseAndSetIfChanged(ref _proofRequestName, value);
        }

        public ProofRequest ProofRequest { get; set; }

        public ObservableCollection<ProofRequestAttributeViewModel> RequestedAttributes
        {
            get => _requestedAttributes;
            set => this.RaiseAndSetIfChanged(ref _requestedAttributes, value);
        }


        #endregion
    }
}
