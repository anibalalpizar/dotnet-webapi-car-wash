using dotnet_mvc_car_wash.Models;
using dotnet_mvc_car_wash.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_mvc_car_wash.Controllers
{
    public class LavadoController : Controller
    {
        private readonly IServiceCarWash serviceCarWash;

        public LavadoController(IServiceCarWash serviceCarWash)
        {
            this.serviceCarWash = serviceCarWash;
        }

        // GET: LavadoController
        public async Task<ActionResult> Index(string searchTerm)
        {
            try
            {
                List<CarWash> list = await serviceCarWash.Get();

                ViewBag.SearchTerm = searchTerm;
                ViewBag.LavadoCount = list.Count;
                ViewBag.FilteredCount = list.Count;

                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading car washes: " + ex.Message;
                return View(new List<CarWash>());
            }
        }

        // GET: LavadoController/Details/5
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

        // GET: LavadoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LavadoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CarWash carWash)
        {
            try
            {
                if (carWash.WashType == Models.Enums.WashType.LaJoya && (!carWash.PricetoAgree.HasValue || carWash.PricetoAgree <= 0))
                {
                    ModelState.AddModelError("PricetoAgree", "You must specify a price for the 'La Joya' wash.");
                }

                if (ModelState.IsValid)
                {
                    bool success = await serviceCarWash.Save(carWash);
                    if (success)
                    {
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

            return View(carWash);
        }

        // GET: LavadoController/Edit/5
        public async Task<ActionResult> Edit(string id)
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
                ViewBag.ErrorMessage = "Error loading car wash for editing: " + ex.Message;
                return View();
            }
        }

        // POST: LavadoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, CarWash carWash)
        {
            try
            {
                if (carWash.WashType == Models.Enums.WashType.LaJoya && (!carWash.PricetoAgree.HasValue || carWash.PricetoAgree <= 0))
                {
                    ModelState.AddModelError("PricetoAgree", "You must specify a price for the 'La Joya' wash.");
                }

                if (ModelState.IsValid)
                {
                    carWash.IdCarWash = id; // Ensure ID doesn't change
                    bool success = await serviceCarWash.Update(carWash);
                    if (success)
                    {
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

            return View(carWash);
        }

        // GET: LavadoController/Delete/5
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

        // POST: LavadoController/Delete/5
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
    }
}