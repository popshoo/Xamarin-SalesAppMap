using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SalesApp.Models;
using SalesApp.Services.Address;
using SalesApp.Services.Settings;
using SalesApp.ViewModels.Base;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;

namespace SalesApp.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IAddressService _addressService;

        private string _currentLocation;
        public string CurrentLocation
        {
            get => _currentLocation;
            set
            {
                _currentLocation = value;
                RaisePropertyChanged(() => CurrentLocation);
            }
        }


        private AddressBuckets _addressBuckets;
        public AddressBuckets AddressBuckets
        {
            get => _addressBuckets;
            set
            {
                _addressBuckets = value;
                RaisePropertyChanged(() => AddressBuckets);
            }
        }

        public string RecentAddressesDisplay => string.Format("{0} Addresses", AddressBuckets?.RecentAddressesCount);
        public string RecentInteractionsDisplay => string.Format("{0} Interactions", AddressBuckets?.RecentInteractionsCount);
        public string RecentSalesDisplay => string.Format("{0} Sales", AddressBuckets?.SalesAddressesCount);
        public string RecentPromisingDisplay => string.Format("{0} Promising", AddressBuckets?.PromisingAddressesCount);


        public ICommand GotoRecentAddressesCommand => new Command(GotoRecentAddressesAsync);
        public ICommand GotoRecentSalesCommand => new Command(GotoRecentSalesAsync);
        public ICommand GotoRecentPromisingCommand => new Command(GotoRecentPromisingAsync);



        public DashboardViewModel(ISettingsService settingsService, IAddressService addressServce)
        {
            _settingsService = settingsService;
            _addressService = addressServce;
            IsBusy = true;
        }


        public override async Task InitializeAsync(object navigationData)
        {
            this.IsBusy = true;

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
            }

            this.CurrentLocation = _settingsService.Location.Display;

            if (!String.IsNullOrEmpty(_settingsService.Location.Zip))
            {
                AddressBuckets = await _addressService.GetAddressBucketsAsync(_settingsService.Location.Zip);
                RaisePropertyChanged(() => RecentAddressesDisplay);
                RaisePropertyChanged(() => RecentInteractionsDisplay);
                RaisePropertyChanged(() => RecentSalesDisplay);
                RaisePropertyChanged(() => RecentPromisingDisplay);
            }
            else
            {
                await DialogService.ShowAlertAsync("Cannot get recent activity", "Error", "Ok");
            }

            this.IsBusy = false;

        }


        public async void GotoRecentAddressesAsync()
        {
            var data = new DashboardData
            {
                Addresses = this.AddressBuckets.Addresses,
                Header = "Addresses With Recent Interactions"
            };
            await NavigationService.NavigateToAsync<SalesManagementViewModel>(data);
        }

        public async void GotoRecentSalesAsync()
        {
            var data = new DashboardData
            {
                Addresses = this.AddressBuckets.Sales,
                Header = "Addresses With Recent Sales"
            };
            await NavigationService.NavigateToAsync<SalesManagementViewModel>(data);
        }

        public async void GotoRecentPromisingAsync()
        {
            var data = new DashboardData
            {
                Addresses = this.AddressBuckets.Sales,
                Header = "Addresses Recently Promising"
            };
            await NavigationService.NavigateToAsync<SalesManagementViewModel>(data);
        }
    }
}

