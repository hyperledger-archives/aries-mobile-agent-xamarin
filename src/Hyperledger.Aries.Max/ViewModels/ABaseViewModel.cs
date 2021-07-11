﻿using System.Threading.Tasks;
using Acr.UserDialogs;
using Hyperledger.Aries.Max.Services.Interfaces;
using ReactiveUI;

namespace Hyperledger.Aries.Max.ViewModels
{
    public abstract class ABaseViewModel : ReactiveObject, IABaseViewModel
    {
        protected readonly IUserDialogs DialogService;
        protected readonly INavigationService NavigationService;

        protected ABaseViewModel(string name, IUserDialogs userDialogs, INavigationService navigationService)
        {
            Name = name;
            DialogService = userDialogs;
            NavigationService = navigationService;
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        public virtual Task InitializeAsync(object navigationData)
        {
            return Task.FromResult(false);
        }
    }
}
