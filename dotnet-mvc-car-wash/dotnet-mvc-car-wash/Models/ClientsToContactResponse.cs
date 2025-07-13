namespace dotnet_mvc_car_wash.Models
{
    public class ClientsToContactResponse
    {
        public string Message { get; set; } = "";
        public List<CustomerReportDto> Clients { get; set; } = new List<CustomerReportDto>();
        public int TotalClients { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class CustomerReportDto
    {
        public string IdNumber { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Province { get; set; } = "";
        public string Canton { get; set; } = "";
        public string District { get; set; } = "";
        public string ExactAddress { get; set; } = "";
        public string WashPreference { get; set; } = "";
        public List<VehicleReportDto> Vehicles { get; set; } = new List<VehicleReportDto>();
        public int TotalVehicles { get; set; }
        public int VehiclesNeedingWash { get; set; }
        public DateTime RecommendedContactDate { get; set; }
        public int Priority { get; set; }
        public double AverageDaysSinceLastWash { get; set; }
    }

    public class VehicleReportDto
    {
        public string LicensePlate { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public string Color { get; set; } = "";
        public DateTime? LastWashDate { get; set; }
        public int DaysSinceLastWash { get; set; }
        public bool NeedsContact { get; set; }
        public string? LastWashType { get; set; }
        public bool HasNanoCeramicTreatment { get; set; }
    }

    public class WashStatistics
    {
        public int TotalCarWashes { get; set; }
        public int CarWashesLastMonth { get; set; }
        public int CarWashesLastWeek { get; set; }
        public List<WashTypeCount> WashTypeDistribution { get; set; } = new List<WashTypeCount>();
        public List<StatusCount> StatusDistribution { get; set; } = new List<StatusCount>();
        public decimal RevenueLastMonth { get; set; }
        public double AverageServiceInterval { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class WashTypeCount
    {
        public string WashType { get; set; } = "";
        public int Count { get; set; }
    }

    public class StatusCount
    {
        public string Status { get; set; } = "";
        public int Count { get; set; }
    }

    public class CustomerActivity
    {
        public CustomerInfo Customer { get; set; } = new CustomerInfo();
        public int TotalVehicles { get; set; }
        public int TotalWashes { get; set; }
        public DateTime? LastWashDate { get; set; }
        public string? FavoriteWashType { get; set; }
        public decimal TotalSpent { get; set; }
        public List<WashHistoryItem> WashHistory { get; set; } = new List<WashHistoryItem>();
        public List<VehicleDetail> VehicleDetails { get; set; } = new List<VehicleDetail>();
        public DateTime GeneratedAt { get; set; }
    }

    public class CustomerInfo
    {
        public string IdNumber { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string WashPreference { get; set; } = "";
    }

    public class WashHistoryItem
    {
        public string IdCarWash { get; set; } = "";
        public string VehicleLicensePlate { get; set; } = "";
        public string WashType { get; set; } = "";
        public decimal TotalPrice { get; set; }
        public DateTime CreationDate { get; set; }
        public string WashStatus { get; set; } = "";
        public string? Observations { get; set; }
    }

    public class VehicleDetail
    {
        public string LicensePlate { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public string Color { get; set; } = "";
        public DateTime? LastWash { get; set; }
        public int WashCount { get; set; }
    }
}
