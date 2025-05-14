using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using smartmetercms.Data;
using smartmetercms.Models;

namespace smartmetercms.Controllers
{
    public class IntervalEnergyUsageController : Controller
    {
        private readonly smartmetercmsContext _context;

        public IntervalEnergyUsageController(smartmetercmsContext context)
        {
            _context = context;
        }

        // GET: IntervalEnergyUsage
        public async Task<IActionResult> Index()
        {
            var smartmetercmsContext = _context.IntervalEnergyUsage.Include(i => i.User);
            return View(await smartmetercmsContext.ToListAsync());
        }

        // GET: IntervalEnergyUsage/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var intervalEnergyUsage = await _context.IntervalEnergyUsage
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (intervalEnergyUsage == null)
            {
                return NotFound();
            }

            return View(intervalEnergyUsage);
        }

        // GET: IntervalEnergyUsage/Create
        public IActionResult Create()
        {
            ViewData["MeterID"] = new SelectList(_context.User, "MeterID", "MeterID");
            return View();
        }

        // POST: IntervalEnergyUsage/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MeterID,Timestamp,EnergyUsed")] IntervalEnergyUsage intervalEnergyUsage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(intervalEnergyUsage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MeterID"] = new SelectList(_context.User, "MeterID", "MeterID", intervalEnergyUsage.MeterID);
            return View(intervalEnergyUsage);
        }

        // GET: IntervalEnergyUsage/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var intervalEnergyUsage = await _context.IntervalEnergyUsage.FindAsync(id);
            if (intervalEnergyUsage == null)
            {
                return NotFound();
            }
            ViewData["MeterID"] = new SelectList(_context.User, "MeterID", "MeterID", intervalEnergyUsage.MeterID);
            return View(intervalEnergyUsage);
        }

        // POST: IntervalEnergyUsage/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MeterID,Timestamp,EnergyUsed")] IntervalEnergyUsage intervalEnergyUsage)
        {
            if (id != intervalEnergyUsage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(intervalEnergyUsage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IntervalEnergyUsageExists(intervalEnergyUsage.Id))
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
            ViewData["MeterID"] = new SelectList(_context.User, "MeterID", "MeterID", intervalEnergyUsage.MeterID);
            return View(intervalEnergyUsage);
        }

        // GET: IntervalEnergyUsage/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var intervalEnergyUsage = await _context.IntervalEnergyUsage
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (intervalEnergyUsage == null)
            {
                return NotFound();
            }

            return View(intervalEnergyUsage);
        }

        // POST: IntervalEnergyUsage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var intervalEnergyUsage = await _context.IntervalEnergyUsage.FindAsync(id);
            if (intervalEnergyUsage != null)
            {
                _context.IntervalEnergyUsage.Remove(intervalEnergyUsage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: IntervalEnergyUsage/DeleteMultiple
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMultiple(int[] selectedIds)
    {
        if (selectedIds == null || selectedIds.Length == 0)
        {
            return RedirectToAction(nameof(Index));
        }

        var itemsToDelete = await _context.IntervalEnergyUsage
            .Where(i => selectedIds.Contains(i.Id))
            .ToListAsync();

        if (itemsToDelete.Any())
        {
            _context.IntervalEnergyUsage.RemoveRange(itemsToDelete);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

        private bool IntervalEnergyUsageExists(int id)
        {
            return _context.IntervalEnergyUsage.Any(e => e.Id == id);
        }
    }
}
