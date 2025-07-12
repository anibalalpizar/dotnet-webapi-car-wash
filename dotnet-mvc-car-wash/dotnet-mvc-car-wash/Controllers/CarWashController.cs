using dotnet_mvc_car_wash.Models;
using dotnet_mvc_car_wash.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace dotnet_mvc_car_wash.Controllers
{
    public class CarWashController : Controller
    {
        private readonly IServiceCarWash serviceCarWash;
        private readonly IServiceCustomer serviceCustomer;
        private readonly IServiceVehicle serviceVehicle;

        public CarWashController(IServiceCarWash serviceCarWash, IServiceCustomer serviceCustomer, IServiceVehicle serviceVehicle)
        {
            this.serviceCarWash = serviceCarWash;
            this.serviceCustomer = serviceCustomer;
            this.serviceVehicle = serviceVehicle;
        }

        // GET: CarWashController
        public async Task<ActionResult> Index(string searchTerm)
        {
            try
            
            {
                List<CarWash> list = await serviceCarWash.Get();
                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading carWash: " + ex.Message;
                return View(new List<CarWash>());
            }
        }

        // GET: CarWashController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            try
            {
                var carWash = await serviceCarWash.GetById(id);
                if (carWash == null)
                {
                    return NotFound();
                }
                return View(carWash);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading car wash details: " + ex.Message;
                return View();
            }
        }

        // GET: CarWashController/Create
        public async Task<ActionResult> Create()
        {
            try
            {
                await LoadSelectLists();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading form data: " + ex.Message;
                return View();
            }
        }

        // POST: CarWashController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CarWash carWash)
        {
            try
            {
                // Validación especial para La Joya wash type
                if (carWash.WashType == Models.Enums.WashType.LaJoya && (!carWash.PricetoAgree.HasValue || carWash.PricetoAgree <= 0))
                {
                    ModelState.AddModelError("PricetoAgree", "You must specify a price for the 'La Joya' wash.");
                }

                // Validar que el vehículo pertenezca al cliente seleccionado
                if (!string.IsNullOrEmpty(carWash.IdClient) && !string.IsNullOrEmpty(carWash.VehicleLicensePlate))
                {
                    var vehicle = await serviceVehicle.GetById(carWash.VehicleLicensePlate);
                    if (vehicle != null && vehicle.CustomerId != carWash.IdClient)
                    {
                        ModelState.AddModelError("VehicleLicensePlate", "The selected vehicle does not belong to the selected customer.");
                    }
                }

                if (ModelState.IsValid)
                {
                    bool success = await serviceCarWash.Save(carWash);
                    if (success)
                    {
                        TempData["SuccessMessage"] = "Car wash created successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Could not create car wash.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating car wash: " + ex.Message);
            }

            await LoadSelectLists();
            return View(carWash);
        }

        // GET: CarWashController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            try
            {
                var carWash = await serviceCarWash.GetById(id);
                if (carWash == null)
                {
                    return NotFound();
                }
                await LoadSelectLists();
                return View(carWash);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading car wash for editing: " + ex.Message;
                return View();
            }
        }

        // POST: CarWashController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, CarWash carWash)
        {
            try
            {
                // Validación especial para La Joya wash type
                if (carWash.WashType == Models.Enums.WashType.LaJoya && (!carWash.PricetoAgree.HasValue || carWash.PricetoAgree <= 0))
                {
                    ModelState.AddModelError("PricetoAgree", "You must specify a price for the 'La Joya' wash.");
                }

                // Validar que el vehículo pertenezca al cliente seleccionado
                if (!string.IsNullOrEmpty(carWash.IdClient) && !string.IsNullOrEmpty(carWash.VehicleLicensePlate))
                {
                    var vehicle = await serviceVehicle.GetById(carWash.VehicleLicensePlate);
                    if (vehicle != null && vehicle.CustomerId != carWash.IdClient)
                    {
                        ModelState.AddModelError("VehicleLicensePlate", "The selected vehicle does not belong to the selected customer.");
                    }
                }

                if (ModelState.IsValid)
                {
                    carWash.IdCarWash = id; // Ensure ID doesn't change
                    bool success = await serviceCarWash.Update(carWash);
                    if (success)
                    {
                        TempData["SuccessMessage"] = "Car wash updated successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Could not update car wash.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating car wash: " + ex.Message);
            }

            await LoadSelectLists();
            return View(carWash);
        }

        // GET: CarWashController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var carWash = await serviceCarWash.GetById(id);
                if (carWash == null)
                {
                    return NotFound();
                }
                return View(carWash);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading car wash for deletion: " + ex.Message;
                return View();
            }
        }

        // POST: CarWashController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, IFormCollection collection)
        {
            try
            {
                bool success = await serviceCarWash.Delete(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Could not delete car wash.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Car wash deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting car wash: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: CarWashController/GetVehiclesByCustomer/{customerId}
        [HttpGet]
        public async Task<JsonResult> GetVehiclesByCustomer(string customerId)
        {
            try
            {
                if (string.IsNullOrEmpty(customerId))
                {
                    return Json(new List<object>());
                }

                var vehicles = await serviceVehicle.Get();
                var customerVehicles = vehicles.Where(v => v.CustomerId == customerId)
                    .Select(v => new { value = v.LicensePlate, text = $"{v.LicensePlate} - {v.Brand} {v.Model}" })
                    .ToList();

                return Json(customerVehicles);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // GET: CarWashController/GetCustomerVehicles/{customerId}
        public async Task<ActionResult> GetCustomerVehicles(string customerId)
        {
            try
            {
                var carWashes = await serviceCarWash.Get();
                var customerCarWashes = carWashes.Where(cw => cw.IdClient == customerId).ToList();

                ViewBag.CustomerName = customerCarWashes.FirstOrDefault()?.Customer?.FullName ?? "Unknown Customer";
                ViewBag.CustomerId = customerId;

                return View(customerCarWashes);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading customer car washes: " + ex.Message;
                return View(new List<CarWash>());
            }
        }

        // GET: CarWashController/GetVehicleHistory/{licensePlate}
        public async Task<ActionResult> GetVehicleHistory(string licensePlate)
        {
            try
            {
                var carWashes = await serviceCarWash.Get();
                var vehicleCarWashes = carWashes.Where(cw => cw.VehicleLicensePlate == licensePlate)
                    .OrderByDescending(cw => cw.CreationDate)
                    .ToList();

                ViewBag.VehicleInfo = vehicleCarWashes.FirstOrDefault()?.Vehicle?.Brand + " " +
                                     vehicleCarWashes.FirstOrDefault()?.Vehicle?.Model ?? "Unknown Vehicle";
                ViewBag.LicensePlate = licensePlate;

                return View(vehicleCarWashes);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading vehicle history: " + ex.Message;
                return View(new List<CarWash>());
            }
        }

        #region Private Helper Methods

        private async Task LoadSelectLists()
        {
            try
            {
                // Load customers
                var customers = await serviceCustomer.Get();
                ViewBag.CustomerSelectList = new SelectList(customers, "IdNumber", "FullName");

                // Load vehicles
                var vehicles = await serviceVehicle.Get();
                ViewBag.VehicleSelectList = new SelectList(vehicles, "LicensePlate", "LicensePlate");

                // Load wash types
                var washTypes = Enum.GetValues(typeof(Models.Enums.WashType))
                    .Cast<Models.Enums.WashType>()
                    .Select(e => new SelectListItem
                    {
                        Value = e.ToString(),
                        Text = e.ToString()
                    });
                ViewBag.WashTypeSelectList = new SelectList(washTypes, "Value", "Text");

                // Load wash statuses
                var washStatuses = Enum.GetValues(typeof(Models.Enums.WashStatus))
                    .Cast<Models.Enums.WashStatus>()
                    .Select(e => new SelectListItem
                    {
                        Value = e.ToString(),
                        Text = e.ToString()
                    });
                ViewBag.WashStatusSelectList = new SelectList(washStatuses, "Value", "Text");
            }
            catch (Exception ex)
            {
                ViewBag.CustomerSelectList = new SelectList(new List<Customer>(), "IdNumber", "FullName");
                ViewBag.VehicleSelectList = new SelectList(new List<Vehicle>(), "LicensePlate", "LicensePlate");
                ViewBag.WashTypeSelectList = new SelectList(new List<SelectListItem>(), "Value", "Text");
                ViewBag.WashStatusSelectList = new SelectList(new List<SelectListItem>(), "Value", "Text");
                ViewBag.ErrorMessage = "Error loading form data: " + ex.Message;
            }
        }

        #endregion
    }
}