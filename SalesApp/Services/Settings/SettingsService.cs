using System;
using System.Collections.Generic;
using System.Text;
using SalesApp.Models;
using SalesApp.Services.Dependency;
using Newtonsoft.Json;

namespace SalesApp.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly ISettingsServiceImplementation _settingsService;
        ISettingsServiceImplementation AppSettings
        {
            get { return _settingsService; }
        }

        public SettingsService(IDependencyService dependencyService)
        {
            _settingsService = dependencyService.Get<ISettingsServiceImplementation>();
        }


        // settings constants
        private const string loggedin_user_id = "LoggedInUser";
        private const string last_tokentime_id = "LastTokenTime";
        private const string current_location_id = "CurrentLocation";

        private const string loggedin_user_default = "";
        private const string last_tokentime_default = "";
        private const string current_location_default = "";


        public DateTime? LastTokenTime
        {
            get
            {
                string data = AppSettings.GetValueOrDefault(last_tokentime_id, last_tokentime_default);
                if (string.IsNullOrEmpty(data))
                {
                    return null;
                }

                return Convert.ToDateTime(data);
            }
            set
            {
                string data = "";
                if (value != null)
                {
                    data = value.Value.ToString();
                }
                AppSettings.AddOrUpdateValue(last_tokentime_id, data);
            }
        }

        public User LoggedInUser
        {
            get
            {
                string data = AppSettings.GetValueOrDefault(loggedin_user_id, loggedin_user_default);
                return String.IsNullOrEmpty(data) ? null : JsonConvert.DeserializeObject<User>(data);
            }
            set
            {
                string data = JsonConvert.SerializeObject(value);
                AppSettings.AddOrUpdateValue(loggedin_user_id, data);
            }
        }


        public CurrentLocation Location
        {
            get
            {
                string data = AppSettings.GetValueOrDefault(current_location_id, current_location_default);
                return String.IsNullOrEmpty(data) ? null : JsonConvert.DeserializeObject<CurrentLocation>(data);
            }
            set
            {
                string data = JsonConvert.SerializeObject(value);
                AppSettings.AddOrUpdateValue(current_location_id, data);
            }
        }
    }
}
