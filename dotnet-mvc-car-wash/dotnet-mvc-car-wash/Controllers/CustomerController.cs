using dotnet_mvc_car_wash.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_mvc_car_wash.Controllers
{
    public class CustomerController : Controller
    {
        private static List<Customer> customers = new List<Customer>();

        // GET: CustomerController
        public ActionResult Index(string searchTerm)
        {
            var filteredCustomers = customers;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Filter customers based on search term
                filteredCustomers = customers.Where(c =>
                    c.IdNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    c.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    c.Province.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    c.Canton.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    c.District.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    c.ExactAddress.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    c.Phone.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    c.WashPreference.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }


            ViewBag.SearchTerm = searchTerm;
            ViewBag.CustomerCount = customers.Count;
            ViewBag.FilteredCount = filteredCustomers.Count;

            return View(filteredCustomers);
        }

        // GET: CustomerController/Details/5
        public ActionResult Details(string id)
        {
            var customer = GetCustomerById(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // GET: CustomerController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CustomerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingCustomer = GetCustomerById(customer.IdNumber);
                    if (existingCustomer == null)
                    {
                        customers.Add(customer);
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Customer with this ID already exists.");
                    }
                }
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while creating the customer.");
            }
            return View(customer);
        }

        // GET: CustomerController/Edit/5
        public ActionResult Edit(string id)
        {
            var customer = GetCustomerById(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: CustomerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    customer.IdNumber = id; // Ensure the ID remains the same
                    bool success = UpdateCustomer(customer);
                    if (success)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "An error occurred while updating the customer.");
                    }
                }
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while updating the customer.");
            }
            return View(customer);
        }

        // GET: CustomerController/Delete/5
        public ActionResult Delete(string id)
        {
            var customer = GetCustomerById(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: CustomerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, IFormCollection collection)
        {
            try
            {
                bool success = DeleteCustomer(id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        private Customer GetCustomerById(string id)
        {
            Customer customer = null;
            foreach (var cum in customers)
            {
                if (cum.IdNumber == id)
                {
                    customer = cum;
                    break;
                }
            }
            return customer;
        }

        private bool UpdateCustomer(Customer updatedCustomer)
        {
            bool success = false;
            try
            {
                for (int i = 0; i < customers.Count; i++)
                {
                    if (customers[i].IdNumber == updatedCustomer.IdNumber)
                    {
                        customers[i] = updatedCustomer;
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

        private bool DeleteCustomer(string id)
        {
            bool success = false;
            try
            {
                var customerToRemove = GetCustomerById(id);
                if (customerToRemove != null)
                {
                    customers.Remove(customerToRemove);
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
