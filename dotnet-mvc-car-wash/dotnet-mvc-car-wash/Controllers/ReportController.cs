using dotnet_mvc_car_wash.Models;
using dotnet_mvc_car_wash.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace dotnet_mvc_car_wash.Controllers
{
    public class ReportController : Controller
    {

        private readonly IServiceReport serviceReport;
        private readonly IServiceCustomer serviceCustomer;

        public ReportController(IServiceReport serviceReport, IServiceCustomer serviceCustomer)
        {
            this.serviceReport = serviceReport;
            this.serviceCustomer = serviceCustomer;
        }

        // GET: ReportController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ReportController/ClientsToContact
        public async Task<ActionResult> ClientsToContact()
        {
            try
            {
                var report = await serviceReport.GetClientsToContact();
                return View(report);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading clients to contact report: " + ex.Message;
                return View(new ClientsToContactResponse());
            }
        }

        // GET: ReportController/WashStatistics
        public async Task<ActionResult> WashStatistics()
        {
            try
            {
                var statistics = await serviceReport.GetWashStatistics();
                return View(statistics);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading wash statistics: " + ex.Message;
                return View(new WashStatistics());
            }
        }

        // GET: ReportController/CustomerActivity
        public async Task<ActionResult> CustomerActivity()
        {
            try
            {
                await LoadCustomerSelectList();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading customers: " + ex.Message;
                return View();
            }
        }

        // POST: ReportController/CustomerActivity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CustomerActivity(string customerId)
        {
            try
            {
                if (string.IsNullOrEmpty(customerId))
                {
                    ModelState.AddModelError("", "Please select a customer.");
                    await LoadCustomerSelectList();
                    return View();
                }

                var activity = await serviceReport.GetCustomerActivity(customerId);
                return View("CustomerActivityResult", activity);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading customer activity: " + ex.Message;
                await LoadCustomerSelectList();
                return View();
            }
        }

        // GET: ReportController/CustomerActivityDetails/{customerId}
        public async Task<ActionResult> CustomerActivityDetails(string customerId)
        {
            try
            {
                var activity = await serviceReport.GetCustomerActivity(customerId);
                return View(activity);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading customer activity details: " + ex.Message;
                return View(new CustomerActivity());
            }
        }

        // Action para exportar reportes (futuro)
        public async Task<ActionResult> ExportClientsToContact()
        {
            try
            {
                var report = await serviceReport.GetClientsToContact();
                // Aquí podrías implementar la exportación a Excel, PDF, etc.
                TempData["InfoMessage"] = "Export functionality coming soon.";
                return RedirectToAction("ClientsToContact");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error exporting report: " + ex.Message;
                return RedirectToAction("ClientsToContact");
            }
        }

        private async Task LoadCustomerSelectList()
        {
            try
            {
                var customers = await serviceCustomer.Get();
                ViewBag.CustomerSelectList = new SelectList(customers, "IdNumber", "FullName");
            }
            catch (Exception ex)
            {
                ViewBag.CustomerSelectList = new SelectList(new List<Customer>(), "IdNumber", "FullName");
                ViewBag.ErrorMessage = "Error loading customers: " + ex.Message;
            }
        }
    }
}
