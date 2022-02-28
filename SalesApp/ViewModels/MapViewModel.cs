using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SalesApp.Models;
using SalesApp.Services.Address;
using SalesApp.Services.Settings;
using SalesApp.Services.Zones;
using SalesApp.ViewModels.Base;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.Bindings;


namespace SalesApp.ViewModels
{
    public class MapViewModel : ViewModelBase
    {
        private readonly IZoneService _zoneService;
        private readonly IAddressService _addressService;

        private MapSpan _visibleRegion;
        public MapSpan VisibleRegion
        {
            get => _visibleRegion;
            set
            {
                _visibleRegion = value;
                RaisePropertyChanged(() => VisibleRegion);
            }
        }
        public MoveToRegionRequest MoveToRegionRequest { get; } = new MoveToRegionRequest();


        private ObservableCollection<Pin> _pins;
        public ObservableCollection<Pin> Pins
        {
            get => _pins;
            set
            {
                _pins = value;
                RaisePropertyChanged(() => Pins);
            }
        }


        public MapViewModel(IZoneService zoneService, IAddressService addressService)
        {
            _zoneService = zoneService;
            _addressService = addressService;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            Coordinates defaultCoordinates = _zoneService.GetAssignedLatitudeLongitude();
            VisibleRegion = MapSpan.FromCenterAndRadius(
                new Position(defaultCoordinates.Latitude, defaultCoordinates.Longitude),
                Distance.FromMiles(0.25));

            MoveToRegionRequest.MoveToRegion(VisibleRegion);

            var addresses =
               await _addressService.GetNearbyAddressesAsync(defaultCoordinates.Latitude, defaultCoordinates.Longitude);

            foreach (Address a in addresses)
            {
                Pins.Add(new Pin
                {
                    Type = PinType.Place,
                    Address = a.Address1,
                    Label = a.AddressDisplay,
                    Position = new Position(a.Latitude, a.Longitude)
                });
            }

            RaisePropertyChanged(() => Pins);
        }
    }
}
