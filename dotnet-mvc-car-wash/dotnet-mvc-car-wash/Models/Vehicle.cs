using System.ComponentModel.DataAnnotations;

namespace dotnet_mvc_car_wash.Models
{
    public class Vehicle
    {
        [Required]
        [Display(Name = "License Plate")]
        public string LicensePlate { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public string Traction { get; set; }

        [Required]
        public string Color { get; set; }

        [Display(Name = "Last Service Date")]
        public DateTime? LastServiceDate { get; set; }

        [Display(Name = "Nano Ceramic Treatment")]
        public bool HasNanoCeramicTreatment { get; set; }

        [Required]
        [Display(Name = "Customer")]
        public string CustomerId { get; set; }

        public Customer? Customer { get; set; }
    }
}
