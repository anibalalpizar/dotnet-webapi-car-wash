using dotnet_webapi_car_wash.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace dotnet_webapi_car_wash.Models
{
    public class CarWash
    {
        [Required]
        [Display(Name = "Car Wash ID")]
        public string IdCarWash { get; set; }

        [Required]
        [Display(Name = "Vehicle License Plate")]
        public string VehicleLicensePlate { get; set; }

        [Required]
        [Display(Name = "Client ID")]
        public string IdClient { get; set; }

        [Required]
        [Display(Name = "Employee ID")]
        public string IdEmployee { get; set; }

        [Required]
        [Display(Name = "Wash Type")]
        public WashType WashType { get; set; }

        [Display(Name = "Base Price")]
        public decimal BasePrice { get; set; }

        [Display(Name = "Price to Agree")]
        public decimal? PricetoAgree { get; set; }

        [Display(Name = "IVA (13%)")]
        public decimal IVA { get; set; }

        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }

        [Required]
        [Display(Name = "Wash Status")]
        public WashStatus WashStatus { get; set; }

        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; }

        [Display(Name = "Observations")]
        public string? Observations { get; set; }

        public CarWash()
        {
            CreationDate = DateTime.Now;
            WashStatus = WashStatus.Scheduled;
        }

        public void CalculatePrices()
        {
            if (WashType == WashType.LaJoya)
            {
                BasePrice = PricetoAgree ?? 0m;
            }
            else
            {
                BasePrice = WashType switch
                {
                    WashType.Basic => 8000m,
                    WashType.Premium => 12000m,
                    WashType.Deluxe => 20000m,
                    _ => 0m
                };
            }

            IVA = BasePrice * 0.13m;
            TotalPrice = BasePrice + IVA;
        }

        public string GetTipoLavadoDescripcion()
        {
            return WashType switch
            {
                WashType.Basic => "Washing, vacuuming y waxing",
                WashType.Premium => "Washing, vacuuming, waxing and deep cleaning of seats",
                WashType.Deluxe => "Washing, vacuuming, and waxing, deep seat cleaning, and paint correction. Optional nanoceramic-treated car wash products.",
                WashType.LaJoya => "Includes all the details to be agreed upon, polishing, hydrophobic treatments, among others.",
                _ => "Description not available"
            };
        }
    }
}