namespace dotnet_webapi_car_wash.Models
{
    public class CustomerReportDto
    {
        public string IdNumber { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Province { get; set; }
        public string Canton { get; set; }
        public string District { get; set; }
        public string ExactAddress { get; set; }
        public string WashPreference { get; set; }
        public List<VehicleReportDto> Vehicles { get; set; } = new List<VehicleReportDto>();
        public int TotalVehicles { get; set; }
        public int VehiclesNeedingWash { get; set; }
        public DateTime RecommendedContactDate { get; set; }
        public int Priority { get; set; }
        public double AverageDaysSinceLastWash { get; set; }
    }

    public class VehicleReportDto
    {
        public string LicensePlate { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public DateTime? LastWashDate { get; set; }
        public int DaysSinceLastWash { get; set; }
        public bool NeedsContact { get; set; }
        public string LastWashType { get; set; }
        public bool HasNanoCeramicTreatment { get; set; }
    }
}
