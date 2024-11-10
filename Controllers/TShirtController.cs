using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderingSystem.Data;
using OrderingSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderingSystem.Controllers
{
    public class TShirtController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TShirtController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult GenerateReceipt()
        {
            // Fetch ordered T-shirts from the database
            var orderedTShirts = _context.OrderedTShirts.ToList();

            // Calculate the receipt details here if needed and pass it to the view
            return View(orderedTShirts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PurchaseAll()
        {
            var orderedTShirts = await _context.OrderedTShirts.ToListAsync();

            // Optional: Process the purchase logic here (e.g., save to order history, clear the ordered items)
            // For example, you might want to remove the ordered items after purchase
            // _context.OrderedTShirts.RemoveRange(orderedTShirts);
            // await _context.SaveChangesAsync();

            // Return the GenerateReceipt view with the ordered T-shirts
            return View("GenerateReceipt", orderedTShirts);
        }

        // Other actions like Index, Create, Edit, etc., remain unchanged
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

            var orderedTShirt = await _context.OrderedTShirts.FindAsync(id);
            if (orderedTShirt == null) return NotFound();

            return View("Edit", orderedTShirt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderedTShirt orderedTShirt)
        {
            if (id != orderedTShirt.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(orderedTShirt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(OrderedItems));
            }

            return View("Edit", orderedTShirt);
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

        public async Task<IActionResult> OrderedItems()
        {
            var orderedTShirts = await _context.OrderedTShirts.ToListAsync();
            return View(orderedTShirts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(int id)
        {
            var tShirt = await _context.TShirts.FindAsync(id);
            if (tShirt == null) return NotFound();

            var orderedTShirt = new OrderedTShirt
            {
                Product = tShirt.Product,
                Quantity = 1,
                Image = tShirt.Image,
                TotalPrice = tShirt.TotalPrice
            };

            _context.OrderedTShirts.Add(orderedTShirt);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(OrderedItems));
        }

        public IActionResult ViewOrderedItems()
        {
            return RedirectToAction(nameof(OrderedItems));
        }

        [HttpPost]
        public async Task<IActionResult> IncrementQuantity(int id)
        {
            var orderedItem = await _context.OrderedTShirts.FindAsync(id);
            if (orderedItem == null) return NotFound();

            orderedItem.Quantity++;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(OrderedItems));
        }

        [HttpPost]
        public async Task<IActionResult> DecrementQuantity(int id)
        {
            var orderedItem = await _context.OrderedTShirts.FindAsync(id);
            if (orderedItem == null) return NotFound();

            if (orderedItem.Quantity > 0)
                orderedItem.Quantity--;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(OrderedItems));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrderedItem(int id)
        {
            var orderedItem = await _context.OrderedTShirts.FindAsync(id);
            if (orderedItem == null) return NotFound();

            _context.OrderedTShirts.Remove(orderedItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(OrderedItems));
        }
    }
}
