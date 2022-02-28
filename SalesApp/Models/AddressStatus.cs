using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SalesApp.Models
{
    public class AddressStatus : ModelBase
    {
        public long Id { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string MapIcon
        {
            get
            {
                if (this.Icon == "ic_money_green.png")
                {
                    return "ic_money_green_map.png";
                }
                if (this.Icon == "ic_warning_red.png")
                {
                    return "ic_warning_red_map.png";
                }

                return "ic_info_outline_blue_map.png";

            }
        }

        [JsonIgnore]
        public List<Address> Addresses { get; set; }


        public AddressStatus()
        {

        }
    }
}
