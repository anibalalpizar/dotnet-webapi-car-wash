using dotnet_mvc_car_wash.Models;
using dotnet_mvc_car_wash.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_mvc_car_wash.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IServiceEmployee serviceEmployee;

        public EmployeeController(IServiceEmployee serviceEmployee)
        {
            this.serviceEmployee = serviceEmployee;
        }

        // GET: EmployeeController
        public async Task<ActionResult> Index(string searchTerm)
        {
            try
            {
                List<Employee> list = await serviceEmployee.Get();
                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading employees: " + ex.Message;
                return View(new List<Employee>());
            }
        }

        // GET: EmployeeController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            try
            {
                var employee = await serviceEmployee.GetById(id);
                if (employee == null)
                {
                    return NotFound();
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading employee details: " + ex.Message;
                return View();
            }
        }

        // GET: EmployeeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Generate a new ID (you might want to use a more sophisticated ID generation)
                    employee.Id = "EMP" + DateTime.Now.Ticks.ToString().Substring(10);

                    bool success = await serviceEmployee.Save(employee);
                    if (success)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Could not create employee.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating employee: " + ex.Message);
            }
            return View(employee);
        }

        // GET: EmployeeController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            try
            {
                var employee = await serviceEmployee.GetById(id);
                if (employee == null)
                {
                    return NotFound();
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading employee for editing: " + ex.Message;
                return View();
            }
        }

        // POST: EmployeeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    employee.Id = id; // Ensure ID doesn't change
                    bool success = await serviceEmployee.Update(employee);
                    if (success)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Could not update employee.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating employee: " + ex.Message);
            }
            return View(employee);
        }

        // GET: EmployeeController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var employee = await serviceEmployee.GetById(id);
                if (employee == null)
                {
                    return NotFound();
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading employee for deletion: " + ex.Message;
                return View();
            }
        }

        // POST: EmployeeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, IFormCollection collection)
        {
            try
            {
                bool success = await serviceEmployee.Delete(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Could not delete employee.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Employee deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting employee: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}