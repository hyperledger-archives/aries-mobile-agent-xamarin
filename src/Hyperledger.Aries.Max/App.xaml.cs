using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Timers;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Max.Services;
using Hyperledger.Aries.Max.Services.Interfaces;
using Hyperledger.Aries.Max.ViewModels;
using Hyperledger.Aries.Max.ViewModels.Account;
using Hyperledger.Aries.Max.ViewModels.Connections;
using Hyperledger.Aries.Max.ViewModels.CreateInvitation;
using Hyperledger.Aries.Max.ViewModels.Credentials;
using Hyperledger.Aries.Max.ViewModels.Proofs;
using Hyperledger.Aries.Max.Views;
using Hyperledger.Aries.Max.Views.Account;
using Hyperledger.Aries.Max.Views.Connections;
using Hyperledger.Aries.Max.Views.CreateInvitation;
using Hyperledger.Aries.Max.Views.Credentials;
using Hyperledger.Aries.Max.Views.Proofs;
using Hyperledger.Aries.Routing;
using Hyperledger.Aries.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Hyperledger.Aries.Max
{
    public partial class App : Application
    {
        public new static App Current => Application.Current as App;
        public static IContainer Container { get; set; }

        // Timer to check new messages in the configured mediator agent every 10sec
        private readonly Timer timer;
        private static IHost Host { get; set; }

        public App()
        {
            InitializeComponent();

            timer = new Timer
            {
                Enabled = false,
                AutoReset = true,
                Interval = TimeSpan.FromSeconds(10).TotalMilliseconds
            };
            timer.Elapsed += Timer_Elapsed;
        }

        public App(IHost host) : this() => Host = host;

        public static IHostBuilder BuildHost(Assembly platformSpecific = null) =>
            XamarinHost.CreateDefaultBuilder<App>()
                .ConfigureServices((_, services) =>
                {
                    services.AddAriesFramework(builder => builder.RegisterEdgeAgent(
                        options: options =>
                        {
                            options.EndpointUri = "http://localhost:5000";

                            options.WalletConfiguration.StorageConfiguration =
                                new WalletConfiguration.WalletStorageConfiguration
                                {
                                    Path = Path.Combine(
                                        path1: FileSystem.AppDataDirectory,
                                        path2: ".indy_client",
                                        path3: "wallets")
                                };
                            options.WalletConfiguration.Id = "MobileWallet";
                            options.WalletCredentials.Key = "SecretWalletKey";
                            options.RevocationRegistryDirectory = Path.Combine(
                                path1: FileSystem.AppDataDirectory,
                                path2: ".indy_client",
                                path3: "tails");

                            // Available network configurations (see PoolConfigurator.cs):
                            //   sovrin-live
                            //   sovrin-staging
                            //   sovrin-builder
                            //   bcovrin-test
                            options.PoolName = "sovrin-staging";
                        },
                        delayProvisioning: true));

                    services.AddSingleton<IPoolConfigurator, PoolConfigurator>();

                    var containerBuilder = new ContainerBuilder();
                    containerBuilder.RegisterAssemblyModules(typeof(CoreModule).Assembly);
                    if (platformSpecific != null)
                    {
                        containerBuilder.RegisterAssemblyModules(platformSpecific);
                    }

                    containerBuilder.Populate(services);
                    Container = containerBuilder.Build();
                });

        protected override async void OnStart()
        {
            await Host.StartAsync();

            // View models and pages mappings
            var _navigationService = Container.Resolve<INavigationService>();
            _navigationService.AddPageViewModelBinding<MainViewModel, MainPage>();
            _navigationService.AddPageViewModelBinding<ConnectionsViewModel, ConnectionsPage>();
            _navigationService.AddPageViewModelBinding<ConnectionViewModel, ConnectionPage>();
            _navigationService.AddPageViewModelBinding<RegisterViewModel, RegisterPage>();
            _navigationService.AddPageViewModelBinding<AcceptInviteViewModel, AcceptInvitePage>();
            _navigationService.AddPageViewModelBinding<CredentialsViewModel, CredentialsPage>();
            _navigationService.AddPageViewModelBinding<CredentialViewModel, CredentialPage>();
            _navigationService.AddPageViewModelBinding<AccountViewModel, AccountPage>();
            _navigationService.AddPageViewModelBinding<CreateInvitationViewModel, CreateInvitationPage>();

            _navigationService.AddPageViewModelBinding<ProofRequestsViewModel, ProofRequestsPage>();
            _navigationService.AddPageViewModelBinding<ProofRequestViewModel, ProofRequestPage>();
            _navigationService.AddPageViewModelBinding<ProofRequestAttributeViewModel, ProofRequestAttributePage>();

            if (Preferences.Get(AppConstant.LocalWalletProvisioned, false))
            {
                await _navigationService.NavigateToAsync<MainViewModel>();
            }
            else
            {
                await _navigationService.NavigateToAsync<RegisterViewModel>();
            }

            timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Check for new messages with the mediator agent if successfully provisioned
            if (Preferences.Get(AppConstant.LocalWalletProvisioned, false))
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        var context = await Container.Resolve<IAgentProvider>().GetContextAsync();
                        await Container.Resolve<IEdgeClientService>().FetchInboxAsync(context);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                });
            }
        }
        protected override void OnSleep() =>
            // Stop timer when application goes to background
            timer.Enabled = false;

        protected override void OnResume() =>
            // Resume timer when application comes in foreground
            timer.Enabled = true;
    }
}
