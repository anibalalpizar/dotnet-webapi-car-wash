using dotnet_mvc_car_wash.Models;
using dotnet_mvc_car_wash.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_mvc_car_wash.Controllers
{
    public class VehicleController : Controller
    {
        private readonly IServiceVehicle serviceVehicle;

        public VehicleController(IServiceVehicle serviceVehicle)
        {
            this.serviceVehicle = serviceVehicle;
        }

        // GET: VehicleController
        public async Task<ActionResult> Index(string searchTerm)
        {
            try
            {
                List<Vehicle> list = await serviceVehicle.Get();
                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading vehicle: " + ex.Message;
                return View(new List<Vehicle>());
            }
        }

        // GET: VehicleController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            try
            {
                var vehicle = await serviceVehicle.GetById(id);
                if (vehicle == null)
                {
                    return NotFound();
                }
                return View(vehicle);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading vehicle details: " + ex.Message;
                return View();
            }
        }

        // GET: VehicleController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VehicleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Vehicle vehicle)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool success = await serviceVehicle.Save(vehicle);
                    if (success)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Could not create vehicle.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating vehicle: " + ex.Message);
            }
            return View(vehicle);
        }

        // GET: VehicleController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            try
            {
                var vehicle = await serviceVehicle.GetById(id);
                if (vehicle == null)
                {
                    return NotFound();
                }
                return View(vehicle);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading vehicle for editing: " + ex.Message;
                return View();
            }
        }

        // POST: VehicleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, Vehicle vehicle)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    vehicle.LicensePlate = id; // Ensure license plate doesn't change
                    bool success = await serviceVehicle.Update(vehicle);
                    if (success)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Could not update vehicle.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating vehicle: " + ex.Message);
            }
            return View(vehicle);
        }

        // GET: VehicleController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var vehicle = await serviceVehicle.GetById(id);
                if (vehicle == null)
                {
                    return NotFound();
                }
                return View(vehicle);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading vehicle for deletion: " + ex.Message;
                return View();
            }
        }

        // POST: VehicleController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, IFormCollection collection)
        {
            try
            {
                bool success = await serviceVehicle.Delete(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Could not delete vehicle.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Vehicle deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting vehicle: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}