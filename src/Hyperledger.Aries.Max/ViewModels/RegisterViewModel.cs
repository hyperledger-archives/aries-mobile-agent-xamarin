using System;
using System.Diagnostics;
using System.Windows.Input;
using Acr.UserDialogs;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Agents.Edge;
using Hyperledger.Aries.Max.Services;
using Hyperledger.Aries.Max.Services.Interfaces;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Hyperledger.Aries.Max.ViewModels
{
    public class RegisterViewModel : ABaseViewModel
    {
        private readonly IAgentProvider _agentContextProvider;
        private readonly IPoolConfigurator _poolConfigurator;
        private readonly IEdgeProvisioningService _provisioningService;

        public RegisterViewModel(
            IUserDialogs userDialogs,
            INavigationService navigationService,
            IAgentProvider agentProvider,
            IPoolConfigurator poolConfigurator,
            IEdgeProvisioningService provisioningService) : base(
                nameof(RegisterViewModel),
                userDialogs,
                navigationService)
        {
            _agentContextProvider = agentProvider;
            _poolConfigurator = poolConfigurator;
            _provisioningService = provisioningService;
        }

        #region Bindable Commands
        public ICommand CreateWalletCommand => new Command(async () =>
        {
            var dialog = UserDialogs.Instance.Loading("Creating wallet");

            try
            {
                await _poolConfigurator.ConfigurePoolsAsync();
                await _provisioningService.ProvisionAsync();
                Preferences.Set(AppConstant.LocalWalletProvisioned, true);

                await NavigationService.NavigateToAsync<MainViewModel>();
            }
            catch(Exception e)
            {
                UserDialogs.Instance.Alert($"Failed to create wallet: {e.Message}");
                Debug.WriteLine(e);
            }
            finally
            {
                dialog?.Hide();
                dialog?.Dispose();
            }
        });
        #endregion
    }
}
