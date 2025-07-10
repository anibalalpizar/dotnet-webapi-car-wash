using dotnet_mvc_car_wash.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace dotnet_mvc_car_wash.Controllers
{
    public class VehicleController : Controller
    {
        private static List<Vehicle> vehicles = new List<Vehicle>();

        // GET: VehicleController
        public ActionResult Index(string searchTerm)
        {
            var filteredVehicles = vehicles;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Filter vehicles based on search term
                filteredVehicles = vehicles.Where(v =>
                    v.LicensePlate.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Brand.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Model.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Traction.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Color.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (v.LastServiceDate?.ToString("dd/MM/yyyy").Contains(searchTerm) ?? false) ||
                    (v.HasNanoCeramicTreatment.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.VehicleCount = vehicles.Count;
            ViewBag.FilteredCount = filteredVehicles.Count;

            return View(filteredVehicles);
        }

        // GET: VehicleController/Details/5
        public ActionResult Details(string id)
        {
            var vehicle = GetVehicleById(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            return View(vehicle);
        }

        // GET: VehicleController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VehicleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Vehicle vehicle)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingVehicle = GetVehicleById(vehicle.LicensePlate);
                    if (existingVehicle == null)
                    {
                        vehicles.Add(vehicle);
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "A vehicle with that license plate already exists.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                ModelState.AddModelError("", "An error occurred while creating the vehicle: " + ex.Message);
            }
            return View(vehicle);
        }

        // GET: VehicleController/Edit/5
        public ActionResult Edit(string id)
        {
            var vehicle = GetVehicleById(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            return View(vehicle);
        }

        // POST: VehicleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, Vehicle vehicle)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    vehicle.LicensePlate = id; // Ensure the license plate remains the same
                    bool success = UpdateVehicle(vehicle);
                    if (success)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "An error occurred while updating the vehicle.");
                    }

                }
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while updating the vehicle.");
            }
            return View(vehicle);
        }

        // GET: VehicleController/Delete/5
        public ActionResult Delete(string id)
        {
            var vehicle = GetVehicleById(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            return View(vehicle);
        }

        // POST: VehicleController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, IFormCollection collection)
        {
            try
            {
                bool success = DeleteVehicle(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private Vehicle GetVehicleById(string id)
        {
            Vehicle vechicle = null;
            foreach (var veh in vehicles)
            {
                if (veh.LicensePlate == id)
                {
                    vechicle = veh;
                    break;
                }
            }
            return vechicle;
        }

        private bool UpdateVehicle(Vehicle updatedVehicle)
        {
            bool success = false;
            try
            {
                for (int i = 0; i < vehicles.Count; i++)
                {
                    if (vehicles[i].LicensePlate == updatedVehicle.LicensePlate)
                    {
                        vehicles[i] = updatedVehicle;
                        success = true;
                        break;
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        private bool DeleteVehicle(string id)
        {
            bool success = false;
            try
            {
                Vehicle vehicleToRemove = GetVehicleById(id);
                if (vehicleToRemove != null)
                {
                    vehicles.Remove(vehicleToRemove);
                    success = true;
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }
    }
}