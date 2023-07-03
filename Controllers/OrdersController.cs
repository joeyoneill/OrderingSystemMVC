using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc_obj_2.Data;

namespace mvc_obj_2.Controllers
{
    public class OrdersController : Controller
    {
        private readonly TempdbContext _context;

        public OrdersController(TempdbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
              return _context.Orders != null ? 
                          View(await _context.Orders.ToListAsync()) :
                          Problem("Entity set 'TempdbContext.Orders'  is null.");
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Initial null check
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            // Get + Check order
            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            // Get order items
            var orderItems = await _context.OrdersItems.Where(oi => oi.OrderId == id).Include(oi => oi.Item).ToListAsync();

            // Return Model Creation
            var viewModel = new DetailsViewModel {
                order = order,
                orderItems = orderItems
            };

            return View(viewModel);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // GET: Order/EditItems/{orderId}
        public async Task<IActionResult> EditItems(int? id) {

            // null check
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            // Get + check order
            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            // Get All Items from OrdersItems and save to list
            var orderItems = await _context.OrdersItems.Where(oi => oi.OrderId == id).Include(oi => oi.Item).ToListAsync();

            // Return Model Class
            var viewModel = new EditItemsViewModel {
                order = order,
                orderItems = orderItems
            };

            return View(viewModel);
        }

        // GET View for Adding Item to Order
        // GET: Order/AddItem/{orderId}
        public async Task<IActionResult> AddItem(int? id) {

            // null check
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            // Get + check order
            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            // Get All Items in Inventory and save to list for view
            var inv = await _context.Items.ToListAsync();

            // Return Model Class
            var viewModel = new AddItemsViewModel {
                order = order,
                inventory = inv
            };

            // ret
            return View(viewModel);
        }

        // POST: Orders/AddItem/{orderId}
        // Adds Item to Order.orderId
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(int? orderId, int? itemId)
        {
            // initial null check
            if (orderId == null || itemId == null || _context.Orders == null || _context.Items == null){
                return NotFound();
            }

            // Get Order + Item
            var order = await _context.Orders.FindAsync(orderId);
            var item = await _context.Items.FindAsync(itemId);
            
            // null check for db objects
            if (order == null) {
                return NotFound();
            }
            if (item == null) {
                TempData["ErrorMessage"] = "Please Enter a Valid Item Id Number.";
                return Redirect($"/Orders/AddItem/{orderId}");
            }

            // Create OrdersItems Object
            var relationshipObject = new OrdersItem {
                OrderId = order.Id,
                ItemId = item.Id
            };

            // Add Item Price to Subtotal + Update
            order.Subtotal += item.ItemPrice;
            _context.Orders.Update(order);

            // Save Instance to DB
            _context.OrdersItems.Add(relationshipObject);
            await _context.SaveChangesAsync();
            
            // ret
            return Redirect($"/Orders/EditItems/{orderId}");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderName,OrderAddress,Subtotal")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return Redirect($"/Orders/EditItems/{order.Id}");
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderName,OrderAddress,Subtotal")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'TempdbContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                // Get All objects from OrdersItems table
                var orderItems = await _context.OrdersItems.Where(oi => oi.OrderId == id).ToListAsync();

                // remove Objects from OrdersItems
                foreach (var obj in orderItems) {
                    _context.OrdersItems.Remove(obj);
                }

                // remove order
                _context.Orders.Remove(order);
            }
            
            // Save DB changes + redirect
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // POST: Orders/RemoveItemFromOrder/{OrdersItemsId}
        [HttpPost]
        public async Task<IActionResult> RemoveItemFromOrder(int? OrdersItemsId) {

            // initial null check
            if (OrdersItemsId == null || _context.OrdersItems == null || _context.Orders == null || _context.Items == null){
                return NotFound();
            }

            // Get OrderItem
            var orderItem = await _context.OrdersItems.FindAsync(OrdersItemsId);
            
            // null check for db objects
            if (orderItem == null) {
                return NotFound();
            }

            // Get Order + Item
            var order = await _context.Orders.FindAsync(orderItem.OrderId);
            var item = await _context.Items.FindAsync(orderItem.ItemId);

            // null check
            if (order == null || item == null) {
                return NotFound();
            }

            // Update Order Subtotal
            order.Subtotal -= item.ItemPrice;
            _context.Orders.Update(order);

            // Remove OrderItems Object by ID
            _context.OrdersItems.Remove(orderItem);

            // Save Changes to DB
            await _context.SaveChangesAsync();

            // return redirect to order page
            return Redirect($"/Orders/EditItems/{order.Id}");
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Class for Details() GET Return Value
    public class DetailsViewModel {
        public required Order order { get; set; }
        public required List<OrdersItem> orderItems { get;set; }
    }

    // Class for EditItems() GET Return Value
    public class EditItemsViewModel {
        public required Order order { get; set; }
        public required List<OrdersItem> orderItems { get;set; }
    }

    // Class for AddItem() GET Return Value
    public class AddItemsViewModel {
        public required Order order { get; set; }
        public required List<Item> inventory { get;set; }

        public Item? item { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
