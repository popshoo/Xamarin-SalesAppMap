using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SalesApp.Models;
using SalesApp.Services.Authentication;
using SalesApp.Services.Settings;
using SalesApp.ViewModels.Base;
using SalesApp.Views;
using Xamarin.Forms;

namespace SalesApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private IAuthenticationService _authenticationService;
        private ISettingsService _settingsService;
        private string _emailAddress;
        public string EmailAddress
        {
            get => _emailAddress;
            set
            {
                _emailAddress = value;
                RaisePropertyChanged(() => EmailAddress);
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                RaisePropertyChanged(() => Password);
            }
        }


        private string _statusMessage;
        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                _statusMessage = value;
                RaisePropertyChanged(() => StatusMessage);
            }
        }

        public ICommand SignInCommand => new Command(async () => await SignInAsync());


        public LoginViewModel(IAuthenticationService authenticationService, ISettingsService settingsService)
        {
            _authenticationService = authenticationService;
            _settingsService = settingsService;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            if (_settingsService.LoggedInUser != null)
            {
                this.EmailAddress = _settingsService.LoggedInUser.Email;
            }

            if (navigationData != null && navigationData is string)
            {
                this.StatusMessage = (string)navigationData;
            }

            await base.InitializeAsync(navigationData);
        }

        public async Task SignInAsync()
        {
            this.StatusMessage = null;

            if (String.IsNullOrEmpty(EmailAddress) || String.IsNullOrEmpty(Password))
            {
                this.StatusMessage = "Please enter your username and password";
                return;
            }

            this.IsBusy = true;

            User user = await _authenticationService.AuthenticateAsync(this.EmailAddress, this.Password);

            if (user == null)
            {
                this.IsBusy = false;
                this.StatusMessage = "Invalid username or password";
            }
            else
            {
                _settingsService.LoggedInUser = user;
                _settingsService.LastTokenTime = DateTime.Now;

                App.Current.MainPage = new MainView();

                await NavigationService.NavigateToAsync<DashboardViewModel>();

                await NavigationService.RemoveLastFromBackStackAsync();
            }
        }
    }
}
