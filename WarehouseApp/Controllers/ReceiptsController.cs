using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WarehouseApp.Data;
using WarehouseApp.Models;

namespace WarehouseApp.Controllers
{
    public class ReceiptsController : Controller
    {
        private readonly AppDbContext _context;

        public ReceiptsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Receipts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Receipt.ToListAsync());
        }

        // GET: Receipts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receipt = await _context.Receipt
                .FirstOrDefaultAsync(m => m.Id == id);
            if (receipt == null)
            {
                return NotFound();
            }

            return View(receipt);
        }

        // GET: Receipts/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Resources = await _context.Resource
                .Where(r => r.State == Enums.RecordState.Active)
                .ToListAsync();

            ViewBag.Units = await _context.Unit
                .Where(u => u.State == Enums.RecordState.Active)
                .ToListAsync();

            return View();
        }

        // POST: Receipts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Number,Date,Items")] Receipt receipt)
        {
            if (await _context.Receipt.AnyAsync(r => r.Number == receipt.Number))
                ModelState.AddModelError("Number", "Документ с таким номером уже существует.");

            if (ModelState.IsValid)
            {
                _context.Add(receipt);
                await _context.SaveChangesAsync();

                foreach (var item in receipt.Items)
                {
                    var balance = await _context.StockBalance
                        .FirstOrDefaultAsync(b => b.ResourceId == item.ResourceId && b.UnitId == item.UnitId);

                    if (balance == null)
                    {
                        balance = new StockBalance
                        {
                            ResourceId = item.ResourceId,
                            UnitId = item.UnitId,
                            Quantity = 0
                        };
                        _context.StockBalance.Add(balance);
                    }
                    balance.Quantity += item.Quantity;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(receipt);
        }

        // GET: Receipts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receipt = await _context.Receipt.FindAsync(id);
            if (receipt == null)
            {
                return NotFound();
            }
            return View(receipt);
        }

        // POST: Receipts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Number,Date,Items")] Receipt receipt)
        {
            if (id != receipt.Id) return NotFound();

            var existingReceipt = await _context.Receipt
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (existingReceipt == null) return NotFound();
         
            if (await _context.Receipt.AnyAsync(r => r.Number == receipt.Number && r.Id != receipt.Id))
                ModelState.AddModelError("Number", "Документ с таким номером уже существует.");

            if (!ModelState.IsValid) return View(receipt);
            
            foreach (var newItem in receipt.Items)
            {
                var oldItem = existingReceipt.Items.
                    FirstOrDefault(i => i.ResourceId == newItem.ResourceId && i.UnitId == newItem.UnitId);

                var balance = await _context.StockBalance
                    .FirstOrDefaultAsync(b => b.ResourceId == newItem.ResourceId && b.UnitId == newItem.UnitId);

                if (balance == null)
                {
                    balance = new StockBalance
                    {
                        ResourceId = newItem.ResourceId,
                        UnitId = newItem.UnitId,
                        Quantity = 0
                    };
                    _context.StockBalance.Add(balance);
                }

                decimal oldQty = oldItem?.Quantity ?? 0;
                decimal diff = newItem.Quantity - oldQty;

                if (diff < 0 && balance.Quantity < Math.Abs(diff))
                {
                    ModelState.AddModelError("", $"Недостаточно ресурса {newItem.Resource.Name} на складе для уменьшения количества.");
                    return View(receipt);
                }

                balance.Quantity += diff;
            }

            // Обновляем сам документ и его позиции
            existingReceipt.Number = receipt.Number;
            existingReceipt.Date = receipt.Date;
            existingReceipt.Items = receipt.Items;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Receipts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receipt = await _context.Receipt
                .FirstOrDefaultAsync(m => m.Id == id);
            if (receipt == null)
            {
                return NotFound();
            }

            return View(receipt);
        }

        // POST: Receipts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var receipt = await _context.Receipt
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receipt == null) return NotFound();

            foreach (var item in receipt.Items)
            {
                var balance = await _context.StockBalance
                    .FirstOrDefaultAsync (b => b.ResourceId == item.ResourceId && b.UnitId == item.UnitId);

                if(balance == null || balance.Quantity < item.Quantity)
                {
                    ModelState.AddModelError("", $"Недостаточно ресурса {item.Resource.Name} для удаления документа.");
                    return View(receipt);
                }
            }

            foreach (var item in receipt.Items)
            {
                var balance = await _context.StockBalance
                    .FirstOrDefaultAsync(b => b.ResourceId == item.ResourceId && b.UnitId == item.UnitId);
                balance.Quantity -= item.Quantity;
            }
            
            _context.Receipt.Remove(receipt);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ReceiptExists(int id)
        {
            return _context.Receipt.Any(e => e.Id == id);
        }
    }
}
