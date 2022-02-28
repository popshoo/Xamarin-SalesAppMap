using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using SalesApp.Models;
using SalesApp.Services.Enums;
using SalesApp.Services.Settings;
using SalesApp.ViewModels.Base;
using Xamarin.Forms;
using MenuItem = SalesApp.Models.MenuItem;

namespace SalesApp.ViewModels
{
    public class MenuViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;

        public ICommand ItemSelectedCommand => new Command<MenuItem>(SelectMenuItemAsync);
        public ICommand LogoutCommand => new Command(LogoutAsync);


        private ObservableCollection<MenuItem> _menuItems = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> MenuItems
        {
            get => _menuItems;
            set
            {
                _menuItems = value;
                RaisePropertyChanged(() => MenuItems);
            }
        }


        User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                RaisePropertyChanged(() => CurrentUser);
            }
        }
        

        public MenuViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;

            CurrentUser = _settingsService.LoggedInUser;

            InitMenuItems();
        }


        private void InitMenuItems()
        {
            MenuItems.Add(new MenuItem
            {
                Title = "Dashboard",
                MenuItemType = MenuItemType.Dashboard,
                ViewModelType = typeof(DashboardViewModel),
                IsEnabled = true
            });

            MenuItems.Add(new MenuItem
            {
                Title = "Address Management",
                MenuItemType = MenuItemType.SalesManagement,
                ViewModelType = typeof(SalesManagementViewModel),
                IsEnabled = true
            });

            MenuItems.Add(new MenuItem
            {
                Title = "Settings",
                MenuItemType = MenuItemType.Settings,
                ViewModelType = typeof(SettingsViewModel),
                IsEnabled = true
            });
        }


        private async void SelectMenuItemAsync(MenuItem item)
        {
            if (item.IsEnabled)
            {
                switch (item.MenuItemType)
                {
                    case MenuItemType.Dashboard:
                        await NavigationService.NavigateToAsync<DashboardViewModel>(null);
                        break;
                    case MenuItemType.SalesManagement:
                        await NavigationService.NavigateToAsync<SalesManagementViewModel>(null);
                        break;
                    case MenuItemType.Settings:
                        await NavigationService.NavigateToAsync<SettingsViewModel>(null);
                        break;
                }
            }
        }

        private async void LogoutAsync()
        {
            _settingsService.LoggedInUser = null;
            _settingsService.LastTokenTime = null;
            _settingsService.Location = null;

            await NavigationService.NavigateToAsync<LoginViewModel>();
            await NavigationService.RemoveBackStackAsync();
        }
    }
}
