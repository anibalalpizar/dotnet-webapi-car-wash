using dotnet_mvc_car_wash.Models;
using dotnet_mvc_car_wash.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_mvc_car_wash.Controllers
{
    public class LavadoController : Controller
    {
        private static List<CarWash> carWashs = new List<CarWash>();

        // GET: LavadoController
        public ActionResult Index(string searchTerm)
        {
            var filteredLavados = carWashs;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Filter car wash based on search term
                filteredLavados = carWashs.Where(l =>
                    l.IdCarWash.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    l.VehicleLicensePlate.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    l.IdClient.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    l.IdEmployee.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    l.WashType.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    l.WashStatus.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    l.BasePrice.ToString().Contains(searchTerm) ||
                    l.TotalPrice.ToString().Contains(searchTerm) ||
                    l.CreationDate.ToString("dd/MM/yyyy").Contains(searchTerm) ||
                    (l.Observations?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.LavadoCount = carWashs.Count;
            ViewBag.FilteredCount = filteredLavados.Count;

            return View(filteredLavados);
        }

        // GET: LavadoController/Details/5
        public ActionResult Details(string id)
        {
            var carWash = GetCarWashById(id);
            if (carWash == null)
            {
                return NotFound();
            }
            return View(carWash);
        }

        // GET: LavadoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LavadoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CarWash carWash)
        {
            try
            {
                if (carWash.WashType == WashType.LaJoya && (!carWash.PricetoAgree.HasValue || carWash.PricetoAgree <= 0))
                {
                    ModelState.AddModelError("Price to Agree", "You must specify a price for the 'La Joya' wash.");
                }

                if (ModelState.IsValid)
                {
                    var existingLavado = GetCarWashById(carWash.IdCarWash);
                    if (existingLavado == null)
                    {
                        carWash.CalculatePrices();

                        if (carWash.CreationDate == default(DateTime))
                        {
                            carWash.CreationDate = DateTime.Now;
                        }

                        carWashs.Add(carWash);
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "A wash with that ID already exists.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the wash: " + ex.Message);
            }
            return View(carWash);
        }

        // GET: LavadoController/Edit/5
        public ActionResult Edit(string id)
        {
            var carWash = GetCarWashById(id);
            if (carWash == null)
            {
                return NotFound();
            }
            return View(carWash);
        }

        // POST: LavadoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, CarWash carWash)
        {
            try
            {
                if (carWash.WashType == WashType.LaJoya && (!carWash.PricetoAgree.HasValue || carWash.PricetoAgree <= 0))
                {
                    ModelState.AddModelError("PrecioAConvenir", "You must specify a price for the 'La Joya' wash.");
                }

                if (ModelState.IsValid)
                {
                    carWash.IdCarWash = id; // Ensure the ID remains the same

                    carWash.CalculatePrices();

                    bool success = UpdateCarWash(carWash);
                    if (success)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "An error occurred while creating the wash.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the wash: " + ex.Message);
            }
            return View(carWash);
        }

        // GET: LavadoController/Delete/5
        public ActionResult Delete(string id)
        {
            var lavado = GetCarWashById(id);
            if (lavado == null)
            {
                return NotFound();
            }
            return View(lavado);
        }

        // POST: LavadoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, IFormCollection collection)
        {
            try
            {
                bool success = DeleteCarWash(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        private CarWash GetCarWashById(string id)
        {
            CarWash carWash = null;
            foreach (var lav in carWashs)
            {
                if (lav.IdCarWash == id)
                {
                    carWash = lav;
                    break;
                }
            }
            return carWash;
        }

        private bool UpdateCarWash(CarWash updatedCarWash)
        {
            bool success = false;
            try
            {
                for (int i = 0; i < carWashs.Count; i++)
                {
                    if (carWashs[i].IdCarWash == updatedCarWash.IdCarWash)
                    {
                        updatedCarWash.CreationDate = carWashs[i].CreationDate;
                        carWashs[i] = updatedCarWash;
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

        private bool DeleteCarWash(string id)
        {
            bool success = false;
            try
            {
                CarWash lavadoToRemove = GetCarWashById(id);
                if (lavadoToRemove != null)
                {
                    carWashs.Remove(lavadoToRemove);
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