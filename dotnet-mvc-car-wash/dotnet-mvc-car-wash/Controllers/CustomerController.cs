using dotnet_mvc_car_wash.Models;
using dotnet_mvc_car_wash.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_mvc_car_wash.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IServiceCustomer serviceCustomer;

        public CustomerController(IServiceCustomer serviceCustomer)
        {
            this.serviceCustomer = serviceCustomer;
        }

        // GET: CustomerController
        public async Task<ActionResult> Index(string searchTerm)
        {
            try
            {
                List<Customer> list = await serviceCustomer.Get();
                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading customers: " + ex.Message;
                return View(new List<Customer>());
            }
        }

        // GET: CustomerController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            try
            {
                var customer = await serviceCustomer.GetById(id);
                if (customer == null)
                {
                    return NotFound();
                }
                return View(customer);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading customer details: " + ex.Message;
                return View();
            }
        }

        // GET: CustomerController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CustomerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool success = await serviceCustomer.Save(customer);
                    if (success)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Could not create customer.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating customer: " + ex.Message);
            }
            return View(customer);
        }

        // GET: CustomerController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            try
            {
                var customer = await serviceCustomer.GetById(id);
                if (customer == null)
                {
                    return NotFound();
                }
                return View(customer);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading customer for editing: " + ex.Message;
                return View();
            }
        }

        // POST: CustomerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    customer.IdNumber = id; // Ensure ID doesn't change
                    bool success = await serviceCustomer.Update(customer);
                    if (success)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Could not update customer.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating customer: " + ex.Message);
            }
            return View(customer);
        }

        // GET: CustomerController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var customer = await serviceCustomer.GetById(id);
                if (customer == null)
                {
                    return NotFound();
                }
                return View(customer);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading customer for deletion: " + ex.Message;
                return View();
            }
        }

        // POST: CustomerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, IFormCollection collection)
        {
            try
            {
                bool success = await serviceCustomer.Delete(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Could not delete customer.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Customer deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting customer: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}