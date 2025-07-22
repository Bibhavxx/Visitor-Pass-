using Microsoft.AspNetCore.Mvc;
using EventPassGenerator.Models;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace EventPassGenerator.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IConverter _converter;

        public RegistrationController(IConverter converter)
        {
            _converter = converter;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Attendee model)
        {
            if (ModelState.IsValid)
            {
                TempData["Name"] = model.Name;
                TempData["Email"] = model.Email;
                TempData["Role"] = model.Role;
                return RedirectToAction("ICard");
            }
            return View(model);
        }

        public IActionResult ICard()
        {
            TempData.Keep(); // Keep TempData for next requests
            ViewBag.Name = TempData["Name"];
            ViewBag.Email = TempData["Email"];
            ViewBag.Role = TempData["Role"];
            return View();
        }

        [HttpPost]
        public IActionResult DownloadPdf()
        {
            var name = TempData["Name"]?.ToString() ?? "Unknown";
            var email = TempData["Email"]?.ToString() ?? "Unknown";
            var role = TempData["Role"]?.ToString() ?? "Unknown";

            var html = $@"
                <div style='border:2px solid #000;padding:20px;width:300px;font-family:Arial;text-align:center;'>
                    <h2 style='margin-bottom:0;color:navy;'>TechFest 2025</h2>
                    <hr />
                    <p><strong>Name:</strong> {name}</p>
                    <p><strong>Email:</strong> {email}</p>
                    <p><strong>Role:</strong> {role}</p>
                    <hr />
                    <p><em>Please show this I-Card at entry</em></p>
                </div>";

            var pdfDoc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                },
                Objects = {
                    new ObjectSettings() {
                        HtmlContent = html,
                    }
                }
            };

            var file = _converter.Convert(pdfDoc);
            return File(file, "application/pdf", "EventICard.pdf");
        }
    }
}
