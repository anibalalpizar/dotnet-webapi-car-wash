using dotnet_webapi_car_wash.Models;
using dotnet_webapi_car_wash.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_webapi_car_wash.Controllers
{
    [Route("api/Report")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private static List<Customer> customers = CustomerController.customers;
        private static List<Vehicle> vehicles = VehicleController.vehicles;
        private static List<CarWash> carWashs = CarWashController.carWashs;

        // GET: api/Report/clients-to-contact
        [HttpGet("clients-to-contact")]
        public ActionResult<IEnumerable<CustomerReportDto>> GetClientsToContact()
        {
            try
            {
                var clientsToContact = new List<CustomerReportDto>();
                var oneMonthAgo = DateTime.Now.AddMonths(-1);

                foreach (var customer in customers)
                {
                    var customerVehicles = vehicles.Where(v => v.CustomerId == customer.IdNumber).ToList();

                    if (!customerVehicles.Any())
                    {
                        continue;
                    }

                    var vehicleReports = new List<VehicleReportDto>();
                    bool shouldContactClient = false;

                    foreach (var vehicle in customerVehicles)
                    {
                        var lastWash = carWashs
                            .Where(cw => cw.VehicleLicensePlate == vehicle.LicensePlate)
                            .OrderByDescending(cw => cw.CreationDate)
                            .FirstOrDefault();

                        DateTime? lastWashDate = lastWash?.CreationDate;
                        int daysSinceLastWash = 0;
                        bool needsContact = false;

                        if (lastWashDate.HasValue)
                        {
                            daysSinceLastWash = (DateTime.Now - lastWashDate.Value).Days;
                            needsContact = lastWashDate.Value <= oneMonthAgo;
                        }
                        else
                        {
                            needsContact = true;
                            daysSinceLastWash = (DateTime.Now - (vehicle.LastServiceDate ?? DateTime.Now.AddYears(-1))).Days;
                        }

                        if (needsContact)
                        {
                            shouldContactClient = true;
                        }

                        vehicleReports.Add(new VehicleReportDto
                        {
                            LicensePlate = vehicle.LicensePlate,
                            Brand = vehicle.Brand,
                            Model = vehicle.Model,
                            Color = vehicle.Color,
                            LastWashDate = lastWashDate,
                            DaysSinceLastWash = daysSinceLastWash,
                            NeedsContact = needsContact,
                            LastWashType = lastWash?.WashType.ToString(),
                            HasNanoCeramicTreatment = vehicle.HasNanoCeramicTreatment
                        });
                    }

                    if (shouldContactClient)
                    {
                        var recommendedContactDate = CalculateRecommendedContactDate(customer.WashPreference);
                        var priority = CalculatePriority(vehicleReports, customer.WashPreference);

                        clientsToContact.Add(new CustomerReportDto
                        {
                            IdNumber = customer.IdNumber,
                            FullName = customer.FullName,
                            Phone = customer.Phone,
                            Province = customer.Province,
                            Canton = customer.Canton,
                            District = customer.District,
                            ExactAddress = customer.ExactAddress,
                            WashPreference = customer.WashPreference.ToString(),
                            Vehicles = vehicleReports,
                            TotalVehicles = vehicleReports.Count,
                            VehiclesNeedingWash = vehicleReports.Count(v => v.NeedsContact),
                            RecommendedContactDate = recommendedContactDate,
                            Priority = priority,
                            AverageDaysSinceLastWash = vehicleReports.Average(v => v.DaysSinceLastWash)
                        });
                    }
                }

                var sortedClients = clientsToContact
                    .OrderByDescending(c => c.Priority)
                    .ThenByDescending(c => c.AverageDaysSinceLastWash)
                    .ToList();

                if (!sortedClients.Any())
                {
                    return Ok(new
                    {
                        message = "No clients need to be contacted at this time..",
                        clients = new List<CustomerReportDto>(),
                        totalClients = 0,
                        generatedAt = DateTime.Now
                    });
                }

                return Ok(new
                {
                    message = $"Found {sortedClients.Count} clients that need to be contacted.",
                    clients = sortedClients,
                    totalClients = sortedClients.Count,
                    generatedAt = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating client contact report", error = ex.Message });
            }
        }

        // GET: api/Report/wash-statistics
        [HttpGet("wash-statistics")]
        public ActionResult<object> GetWashStatistics()
        {
            try
            {
                var now = DateTime.Now;
                var lastMonth = now.AddMonths(-1);
                var lastWeek = now.AddDays(-7);

                var stats = new
                {
                    TotalCarWashes = carWashs.Count,
                    CarWashesLastMonth = carWashs.Count(cw => cw.CreationDate >= lastMonth),
                    CarWashesLastWeek = carWashs.Count(cw => cw.CreationDate >= lastWeek),
                    WashTypeDistribution = carWashs
                        .GroupBy(cw => cw.WashType)
                        .Select(g => new { WashType = g.Key.ToString(), Count = g.Count() })
                        .OrderByDescending(x => x.Count)
                        .ToList(),
                    StatusDistribution = carWashs
                        .GroupBy(cw => cw.WashStatus)
                        .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                        .ToList(),
                    RevenueLastMonth = carWashs
                        .Where(cw => cw.CreationDate >= lastMonth)
                        .Sum(cw => cw.TotalPrice),
                    AverageServiceInterval = CalculateAverageServiceInterval(),
                    GeneratedAt = now
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating wash statistics", error = ex.Message });
            }
        }

        // GET: api/Report/customer-activity/{customerId}
        [HttpGet("customer-activity/{customerId}")]
        public ActionResult<object> GetCustomerActivity(string customerId)
        {
            try
            {
                var customer = customers.FirstOrDefault(c => c.IdNumber == customerId);
                if (customer == null)
                {
                    return NotFound(new { message = $"Customer with ID '{customerId}' not found" });
                }

                var customerVehicles = vehicles.Where(v => v.CustomerId == customerId).ToList();
                var customerWashes = carWashs.Where(cw => cw.IdClient == customerId).OrderByDescending(cw => cw.CreationDate).ToList();

                var activity = new
                {
                    Customer = new
                    {
                        customer.IdNumber,
                        customer.FullName,
                        customer.Phone,
                        customer.WashPreference
                    },
                    TotalVehicles = customerVehicles.Count,
                    TotalWashes = customerWashes.Count,
                    LastWashDate = customerWashes.FirstOrDefault()?.CreationDate,
                    FavoriteWashType = customerWashes
                        .GroupBy(cw => cw.WashType)
                        .OrderByDescending(g => g.Count())
                        .FirstOrDefault()?.Key.ToString(),
                    TotalSpent = customerWashes.Sum(cw => cw.TotalPrice),
                    WashHistory = customerWashes.Select(cw => new
                    {
                        cw.IdCarWash,
                        cw.VehicleLicensePlate,
                        cw.WashType,
                        cw.TotalPrice,
                        cw.CreationDate,
                        cw.WashStatus,
                        cw.Observations
                    }).ToList(),
                    VehicleDetails = customerVehicles.Select(v => new
                    {
                        v.LicensePlate,
                        v.Brand,
                        v.Model,
                        v.Color,
                        LastWash = customerWashes.FirstOrDefault(cw => cw.VehicleLicensePlate == v.LicensePlate)?.CreationDate,
                        WashCount = customerWashes.Count(cw => cw.VehicleLicensePlate == v.LicensePlate)
                    }).ToList(),
                    GeneratedAt = DateTime.Now
                };

                return Ok(activity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating customer activity report", error = ex.Message });
            }
        }

        private DateTime CalculateRecommendedContactDate(WashPreference preference)
        {
            var now = DateTime.Now;
            return preference switch
            {
                WashPreference.Weekly => now.AddDays(7),
                WashPreference.Biweekly => now.AddDays(14),
                WashPreference.Monthly => now.AddDays(30),
                WashPreference.Other => now.AddDays(90),
                _ => now.AddDays(30)
            };
        }

        private int CalculatePriority(List<VehicleReportDto> vehicles, WashPreference preference)
        {
            var maxDaysSinceWash = vehicles.Max(v => v.DaysSinceLastWash);
            var vehiclesNeedingWash = vehicles.Count(v => v.NeedsContact);

            var basePriority = preference switch
            {
                WashPreference.Weekly => 100,
                WashPreference.Biweekly => 80,
                WashPreference.Monthly => 60,
                WashPreference.Other => 40,
                _ => 50
            };

            var daysPriority = Math.Min(maxDaysSinceWash / 7, 50); 

            var vehiclesPriority = vehiclesNeedingWash * 10;

            return basePriority + daysPriority + vehiclesPriority;
        }

        private double CalculateAverageServiceInterval()
        {
            var customerIntervals = new List<double>();

            foreach (var customer in customers)
            {
                var customerWashes = carWashs
                    .Where(cw => cw.IdClient == customer.IdNumber)
                    .OrderBy(cw => cw.CreationDate)
                    .ToList();

                if (customerWashes.Count >= 2)
                {
                    var intervals = new List<double>();
                    for (int i = 1; i < customerWashes.Count; i++)
                    {
                        var daysBetween = (customerWashes[i].CreationDate - customerWashes[i - 1].CreationDate).TotalDays;
                        intervals.Add(daysBetween);
                    }

                    if (intervals.Any())
                    {
                        customerIntervals.Add(intervals.Average());
                    }
                }
            }

            return customerIntervals.Any() ? customerIntervals.Average() : 0;
        }

    }
}