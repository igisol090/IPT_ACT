using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderingSystem.Data;
using OrderingSystem.Models;
using System.Linq;
using System.Threading.Tasks;

namespace OrderingSystem.Controllers
{
    public class TShirtController : Controller
    {
        private readonly ApplicationDbContext _context;
        private static int _receiptNumber = 1; // For generating a unique receipt number

        public TShirtController(ApplicationDbContext context)
        {
            _context = context;
        }

        // View all available T-shirts
        public async Task<IActionResult> Index()
        {
            return View(await _context.TShirts.ToListAsync());
        }

        // View items in the cart (OrderedTShirts)
        public async Task<IActionResult> OrderedItems()
        {
            var orderedTShirts = await _context.OrderedTShirts.ToListAsync();
            return View(orderedTShirts);
        }

        // Add a T-shirt to the cart (OrderedTShirts)
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(int id, int quantity) // Add quantity to the parameters
        {
            // Find the selected T-shirt based on the ID
            var tShirt = await _context.TShirts.FindAsync(id);
            if (tShirt == null) return NotFound(); // If the T-shirt is not found, return 404

            // Check if the T-shirt is already in the cart
            var existingOrder = await _context.OrderedTShirts
                .FirstOrDefaultAsync(o => o.Product == tShirt.Product && o.Image == tShirt.Image); // Check both Product and Image

            // Update the order if it already exists, or create a new order
            if (existingOrder != null)
            {
                // If the T-shirt is already in the cart, increase the quantity
                existingOrder.Quantity += quantity; // Add the passed quantity to the current quantity
                existingOrder.TotalPrice = existingOrder.Quantity * tShirt.Price; // Recalculate total price
            }
            else
            {
                // If the T-shirt is not in the cart, create a new ordered item with the specified quantity
                var orderedTShirt = new OrderedTShirt
                {
                    Product = tShirt.Product,
                    Quantity = quantity,
                    Image = tShirt.Image,
                    Price = tShirt.Price,
                    TotalPrice = tShirt.Price * quantity // Correct total price calculation
                };
                _context.OrderedTShirts.Add(orderedTShirt);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to the OrderedItems page to show the updated cart
            return RedirectToAction(nameof(OrderedItems));
        }

        // Edit an Ordered T-shirt

            // GET: Edit
            public async Task<IActionResult> Edit(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var orderedTShirt = await _context.OrderedTShirts.FindAsync(id);
                if (orderedTShirt == null)
                {
                    return NotFound();
                }

                return View(orderedTShirt);  // Pass the model to the Edit view
            }

            // POST: Edit
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("Id,Product,Quantity,Price,TotalPrice")] OrderedTShirt orderedTShirt)
            {
                if (id != orderedTShirt.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        // Recalculate the total price
                        orderedTShirt.TotalPrice = orderedTShirt.Quantity * orderedTShirt.Price;

                        // Update the ordered t-shirt in the database
                        _context.Update(orderedTShirt);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!OrderedTShirtExists(orderedTShirt.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    // After the update, redirect to the list of ordered items (or Index page)
                    return RedirectToAction(nameof(Index));
                }

                // If validation failed, return to the Edit view with the current data
                return View(orderedTShirt);
            }

            private bool OrderedTShirtExists(int id)
            {
                return _context.OrderedTShirts.Any(e => e.Id == id);
            }

        // Delete a T-shirt from the cart
        [HttpPost]
        public async Task<IActionResult> DeleteOrderedItem(int id)
        {
            var orderedTShirt = await _context.OrderedTShirts.FindAsync(id);
            if (orderedTShirt == null) return NotFound();

            _context.OrderedTShirts.Remove(orderedTShirt);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(OrderedItems));
        }

        // Increment quantity of a T-shirt in the cart
        [HttpPost]
        public async Task<IActionResult> IncrementQuantity(int id)
        {
            var orderedItem = await _context.OrderedTShirts.FindAsync(id);
            if (orderedItem == null) return NotFound();

            orderedItem.Quantity++;
            // Ensure that TotalPrice is updated using the original unit price
            orderedItem.TotalPrice = orderedItem.Quantity * orderedItem.Price; // Use a stored UnitPrice property

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(OrderedItems));
        }

        // Decrement quantity of a T-shirt in the cart
        [HttpPost]
        public async Task<IActionResult> DecrementQuantity(int id)
        {
            var orderedItem = await _context.OrderedTShirts.FindAsync(id);
            if (orderedItem == null) return NotFound();

            if (orderedItem.Quantity > 1)
            {
                orderedItem.Quantity--;
                // Recalculate TotalPrice based on the unit price and updated quantity
                orderedItem.TotalPrice = orderedItem.Quantity * orderedItem.Price;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(OrderedItems));
        }

        // Purchase all items in the cart
        [HttpPost]
        public async Task<IActionResult> PurchaseAll(string billingName, string billingAddress)
        {
            var orderedItems = await _context.TShirts.ToListAsync(); // Get all ordered T-shirts

            if (!orderedItems.Any()) return RedirectToAction(nameof(OrderedItems));

            var receiptNumber = _receiptNumber++; // Generate a receipt number (simple increment, you might want to use a more sophisticated method)
            var todayDate = DateTime.Now.ToString("MMMM dd, yyyy");

            // Passing the necessary data to the view
            ViewBag.BillingName = billingName;
            ViewBag.BillingAddress = billingAddress;
            ViewBag.ReceiptNumber = receiptNumber;
            ViewBag.TodayDate = todayDate;

            // Do not remove items from the cart, just display the receipt
            // _context.TShirts.RemoveRange(orderedItems); // This line is removed
            // await _context.SaveChangesAsync(); // This line is removed

            // Redirect to the receipt page, passing the ordered T-shirt items
            return View("GenerateReceipt", orderedItems);
        }
    }
}