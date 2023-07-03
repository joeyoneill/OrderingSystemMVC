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
    public class ItemsController : Controller
    {
        private readonly TempdbContext _context;

        public ItemsController(TempdbContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
              return _context.Items != null ? 
                          View(await _context.Items.ToListAsync()) :
                          Problem("Entity set 'TempdbContext.Items'  is null.");
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ItemName,ItemPrice")] Item item)
        {
            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ItemName,ItemPrice")] Item item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.Id))
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
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            // Remove item from orders + OrdersItems
            var orderItems = _context.OrdersItems.Where(oi => oi.ItemId == item.Id).ToList();

            // return model
            var viewModel = new DeleteViewModel {
                item = item,
                ordersList = orderItems
            };

            return View(viewModel);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // null table check
            if (_context.Items == null)
            {
                return Problem("Entity set 'TempdbContext.Items'  is null.");
            }

            // get + check item
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                // Need to remove item from orders + OrdersItems
                // Remove item from orders + OrdersItems
                var orderItems = _context.OrdersItems.Where(oi => oi.ItemId == item.Id).ToList();

                // go through all order items and remove item from order
                foreach (var orderItem in orderItems) {
                    // get order
                    var order = await _context.Orders.FindAsync(orderItem.OrderId);

                    // order null check
                    if (order != null) {

                        // change subtotal
                        order.Subtotal -= item.ItemPrice;

                        // update order
                        _context.Orders.Update(order);
                    }

                    // remove OrdersItems connection
                    _context.OrdersItems.Remove(orderItem);
                }
                

                // Removes Item from DB
                _context.Items.Remove(item);

                // Save DB Changes
                await _context.SaveChangesAsync();
            }
        
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
          return (_context.Items?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

    public class DeleteViewModel {
        public required Item item { get; set; }
        public List<OrdersItem>? ordersList { get; set; }
    }
}
