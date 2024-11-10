using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using OrderingSystem.Data;
using OrderingSystem.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OrderingSystem.Controllers
{
    public class TShirtController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConverter _converter;
        private readonly ICompositeViewEngine _viewEngine;

        // Field to keep track of the receipt number
        private static int _receiptNumber = 1;

        public TShirtController(ApplicationDbContext context, IConverter converter, ICompositeViewEngine viewEngine)
        {
            _context = context;
            _converter = converter;
            _viewEngine = viewEngine;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.TShirts.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TShirt tShirt)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tShirt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tShirt);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tShirt = await _context.TShirts.FindAsync(id);
            if (tShirt == null) return NotFound();

            return View(tShirt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TShirt tShirt)
        {
            if (id != tShirt.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(tShirt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(tShirt);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tShirt = await _context.TShirts.FindAsync(id);
            if (tShirt == null) return NotFound();

            _context.TShirts.Remove(tShirt);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Action to view ordered T-Shirts
        public async Task<IActionResult> OrderedItems()
        {
            var orderedTShirts = await _context.OrderedTShirts.ToListAsync();
            return View(orderedTShirts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PurchaseAll(string billingName, string billingAddress)
        {
            var orderedTShirts = await _context.OrderedTShirts.ToListAsync();
            var todayDate = DateTime.Now.ToString("MMMM dd, yyyy");
            var receiptNumber = _receiptNumber++;

            // Calculate total amount
            var totalAmount = orderedTShirts.Sum(t => t.TotalPrice);

            // Pass data to the view using ViewBag
            ViewBag.BillingName = billingName;
            ViewBag.BillingAddress = billingAddress;
            ViewBag.ReceiptNumber = receiptNumber;
            ViewBag.TodayDate = todayDate;
            ViewBag.TotalAmount = totalAmount;

            return View("GenerateReceipt", orderedTShirts);
        }

        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, true);
                if (viewResult.View == null)
                {
                    throw new ArgumentNullException(nameof(viewResult.View), "View cannot be found.");
                }

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    sw,
                    new HtmlHelperOptions() // Create a new HtmlHelperOptions instance
                );
                viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }
    }
}
