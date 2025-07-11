using System.ComponentModel.DataAnnotations;

namespace dotnet_webapi_car_wash.Models.Enums
{
    public enum WashType
    {
        Basic,
        Premium,
        Deluxe,
        [Display(Name = "La Joya")]
        LaJoya
    }

    public enum WashStatus
    {
        [Display(Name = "In Process")]
        InProgress,
        Billed,
        Scheduled
    }

    public enum WashPreference
    {
        Weekly,
        Biweekly,
        Monthly,
        Other
    }
}