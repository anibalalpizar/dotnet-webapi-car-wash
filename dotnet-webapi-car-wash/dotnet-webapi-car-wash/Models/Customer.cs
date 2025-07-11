using dotnet_webapi_car_wash.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace dotnet_webapi_car_wash.Models
{
    public class Customer
    {
        [Required]
        [Display(Name = "ID Number")]
        public string IdNumber { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        public string Province { get; set; }

        [Required]
        public string Canton { get; set; }

        [Required]
        public string District { get; set; }

        [Required]
        [Display(Name = "Exact Address")]
        public string ExactAddress { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Wash Preference")]
        public WashPreference WashPreference { get; set; }
    }
}
