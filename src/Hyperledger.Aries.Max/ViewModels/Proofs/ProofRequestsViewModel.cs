using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Autofac;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Decorators.Attachments;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Aries.Max.Events;
using Hyperledger.Aries.Max.Extensions;
using Hyperledger.Aries.Max.Services.Interfaces;
using Hyperledger.Aries.Max.Utilities;
using ReactiveUI;
using Xamarin.Forms;

namespace Hyperledger.Aries.Max.ViewModels.Proofs
{
    public class ProofRequestsViewModel : ABaseViewModel
    {
        private readonly IConnectionService _connectionService;
        private readonly IAgentProvider _agentContextProvider;
        private readonly IProofService _proofService;
        private readonly IEventAggregator eventAggregator;
        private readonly ILifetimeScope _scope;

        public ProofRequestsViewModel(IUserDialogs userDialogs,
                                   INavigationService navigationService,
                                   IAgentProvider agentContextProvider,
                                   IProofService proofService,
                                   IConnectionService connectionService,
                                   IEventAggregator eventAggregator,
                                   ILifetimeScope scope) : base("Proof Requests", userDialogs, navigationService)
        {
            _connectionService = connectionService;
            _agentContextProvider = agentContextProvider;
            _proofService = proofService;
            this.eventAggregator = eventAggregator;
            _scope = scope;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            await RefreshProofRequests();

            eventAggregator.GetEventByType<ApplicationEvent>()
                          .Where(_ => _.Type == ApplicationEventType.RefreshProofRequests)
                          .Subscribe(async _ => await RefreshProofRequests());

            await base.InitializeAsync(navigationData);
        }

        public async Task RefreshProofRequests()
        {
            try
            {
                RefreshingProofRequests = true;
                ProofRequests.Clear();

                var agentContext = await _agentContextProvider.GetContextAsync();
                IEnumerable<ProofRecord> proofRequests = (await _proofService.ListRequestedAsync(agentContext));

                IList<ProofRequestViewModel> proofRequestVms = new List<ProofRequestViewModel>();
                foreach (var proofReq in proofRequests)
                {
                    var connection = await _connectionService.GetAsync(agentContext, proofReq.ConnectionId);
                    var proofRequestViewModel = _scope.Resolve<ProofRequestViewModel>(new NamedParameter("proofRecord", proofReq), new NamedParameter("connection", connection));
                    proofRequestVms.Add(proofRequestViewModel);
                }

                ProofRequests.InsertRange(proofRequestVms);
                HasRequests = ProofRequests.Any();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                RefreshingProofRequests = false;
            }
        }

        public async Task ScanProofRequest()
        {
            var expectedFormat = ZXing.BarcodeFormat.QR_CODE;
            var opts = new ZXing.Mobile.MobileBarcodeScanningOptions { PossibleFormats = new List<ZXing.BarcodeFormat> { expectedFormat } };

            var scanner = new ZXing.Mobile.MobileBarcodeScanner();

            var result = await scanner.Scan(opts);
            if (result == null)
                return;

            RequestPresentationMessage presentationMessage;

            try
            {
                presentationMessage = await MessageDecoder.ParseMessageAsync(result.Text) as RequestPresentationMessage
                    ?? throw new Exception("Unknown message type");
            }
            catch (Exception)
            {
                DialogService.Alert("Invalid Proof Request!");
                return;
            }

            if (presentationMessage == null)
                return;

            try
            {
                var request = presentationMessage.Requests?.FirstOrDefault((Attachment x) => x.Id == "libindy-request-presentation-0");
                if (request == null)
                {
                    DialogService.Alert("scanned qr code does not look like a proof request", "Error");
                    return;
                }
                var proofRequest = request.Data.Base64.GetBytesFromBase64().GetUTF8String().ToObject<ProofRequest>();
                if (proofRequest == null)
                    return;

                var proofRequestViewModel = _scope.Resolve<ProofRequestViewModel>(new NamedParameter("proofRequest", proofRequest),
                                                                                  new NamedParameter("requestPresentationMessage", presentationMessage));

                await NavigationService.NavigateToAsync(proofRequestViewModel);
            }
            catch (Exception xx)
            {
                DialogService.Alert(xx.Message);
            }
        }

        #region Bindable Command

        public ICommand ScanProofRequestCommand => new Command(async () => await ScanProofRequest());

        public ICommand SelectProofRequestCommand => new Command<ProofRequestViewModel>(async (proofRequest) =>
        {
            if (proofRequest == null)
                return;

            await NavigationService.NavigateToAsync(proofRequest);
        });

        public ICommand RefreshCommand => new Command(async () => await RefreshProofRequests());

        #endregion

        #region Bindable Properties

        private RangeEnabledObservableCollection<ProofRequestViewModel> _proofRequests = new RangeEnabledObservableCollection<ProofRequestViewModel>();
        public RangeEnabledObservableCollection<ProofRequestViewModel> ProofRequests
        {
            get => _proofRequests;
            set => this.RaiseAndSetIfChanged(ref _proofRequests, value);
        }

        private bool _hasRequests;
        public bool HasRequests
        {
            get => _hasRequests;
            set => this.RaiseAndSetIfChanged(ref _hasRequests, value);
        }

        private bool _refreshingProofRequests;
        public bool RefreshingProofRequests
        {
            get => _refreshingProofRequests;
            set => this.RaiseAndSetIfChanged(ref _refreshingProofRequests, value);
        }
        #endregion

    }
}
