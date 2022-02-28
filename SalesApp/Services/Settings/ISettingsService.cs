using System;
using System.Collections.Generic;
using System.Text;
using SalesApp.Models;

namespace SalesApp.Services.Settings
{
    public interface ISettingsService
    {
        DateTime? LastTokenTime { get; set; }
        User LoggedInUser { get; set; }
        CurrentLocation Location { get; set; }
    }
}
