using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderingSystem.Data;
using OrderingSystem.Models;
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
            var orderedTShirts = await _context.OrderedTShirts.ToListAsync(); // Replace with your actual ordered items logic
            return View(orderedTShirts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(int id)
        {
            var tShirt = await _context.TShirts.FindAsync(id);
            if (tShirt == null) return NotFound();

            // Create a new ordered item
            var orderedTShirt = new OrderedTShirt
            {
                Product = tShirt.Product,
                Quantity = 1, // Default to 1
                Image = tShirt.Image,
                TotalPrice = tShirt.TotalPrice
            };

            _context.OrderedTShirts.Add(orderedTShirt);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(OrderedItems)); // Redirect to the OrderedItems view
        }


        // Increment Quantity
        [HttpPost]
        public async Task<IActionResult> IncrementQuantity(int id)
        {
            var orderedItem = await _context.OrderedTShirts.FindAsync(id);
            if (orderedItem == null) return NotFound();

            orderedItem.Quantity++; // Increment the quantity
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(OrderedItems));
        }

        // Decrement Quantity
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

        // Action to delete an ordered item
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
