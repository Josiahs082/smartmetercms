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
    public class MeterStatusController : Controller
    {
        private readonly smartmetercmsContext _context;

        public MeterStatusController(smartmetercmsContext context)
        {
            _context = context;
        }

        // GET: MeterStatus
        public async Task<IActionResult> Index()
        {
            return View(await _context.MeterStatus.ToListAsync());
        }

        // GET: MeterStatus/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meterStatus = await _context.MeterStatus
                .FirstOrDefaultAsync(m => m.MeterID == id);
            if (meterStatus == null)
            {
                return NotFound();
            }

            return View(meterStatus);
        }

        // GET: MeterStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MeterStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MeterID,Status,LastUpdated")] MeterStatus meterStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(meterStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(meterStatus);
        }

        // GET: MeterStatus/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meterStatus = await _context.MeterStatus.FindAsync(id);
            if (meterStatus == null)
            {
                return NotFound();
            }
            return View(meterStatus);
        }

        // POST: MeterStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MeterID,Status,LastUpdated")] MeterStatus meterStatus)
        {
            if (id != meterStatus.MeterID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(meterStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeterStatusExists(meterStatus.MeterID))
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
            return View(meterStatus);
        }

        // GET: MeterStatus/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meterStatus = await _context.MeterStatus
                .FirstOrDefaultAsync(m => m.MeterID == id);
            if (meterStatus == null)
            {
                return NotFound();
            }

            return View(meterStatus);
        }

        // POST: MeterStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var meterStatus = await _context.MeterStatus.FindAsync(id);
            if (meterStatus != null)
            {
                _context.MeterStatus.Remove(meterStatus);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MeterStatusExists(string id)
        {
            return _context.MeterStatus.Any(e => e.MeterID == id);
        }
    }
}
