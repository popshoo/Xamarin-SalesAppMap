using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Acr.UserDialogs;
using SalesApp.Models;
using SalesApp.Services.Address;
using SalesApp.Services.Settings;
using SalesApp.ViewModels.Base;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;

namespace SalesApp.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private ISettingsService _settingsService;
        private IAddressService _addressService;

        private User _user;
        public User User
        {
            get => _user;
            set
            {
                _user = value;
                RaisePropertyChanged(() => User);
            }
        }


        private CurrentLocation _location;
        public CurrentLocation Location
        {
            get => _location;
            set
            {
                _location = value;
                RaisePropertyChanged(() => Location);
            }
        }


        public ICommand DetectLocationCommand => new Command(DetectLocationAsync);
        public ICommand LogoutCommand => new Command(LogoutAsync);


        public SettingsViewModel(ISettingsService settingsService, IAddressService addressService)
        {
            _settingsService = settingsService;
            _addressService = addressService;

            User = _settingsService.LoggedInUser;
            Location = _settingsService.Location;
        }


        public async void DetectLocationAsync()
        {
            UserDialogs.Instance.ShowLoading("Detecting Location");

            try
            {
                Position position = null;

                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 100;

                position = await locator.GetLastKnownLocationAsync();

                CurrentLocation currentLocation =
                    await _addressService.GetLocationInformationAsync(position.Latitude, position.Longitude);
                currentLocation.Latitude = position.Latitude;
                currentLocation.Longitude = position.Longitude;
                _settingsService.Location = currentLocation;
                RaisePropertyChanged(() => Location);
            }
            catch (Exception ex)
            {
                // this.CurrentLocation = "Current Location: Unknown";
                _settingsService.Location = new CurrentLocation()
                {
                    City = "Pittsburg",
                    State = "KS",
                    Zip = "66762",
                    Latitude = 37.416076,
                    Longitude = -94.672180
                };
                RaisePropertyChanged(() => Location);
            }

            UserDialogs.Instance.HideLoading();
        }

        public async void LogoutAsync()
        {
            _settingsService.LoggedInUser = null;
            _settingsService.LastTokenTime = null;
            _settingsService.Location = null;

            await NavigationService.NavigateToAsync<LoginViewModel>();
            await NavigationService.RemoveBackStackAsync();
        }
    }
}
