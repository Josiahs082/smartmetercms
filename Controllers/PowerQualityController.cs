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
    public class PowerQualityController : Controller
    {
        private readonly smartmetercmsContext _context;

        public PowerQualityController(smartmetercmsContext context)
        {
            _context = context;
        }

        // GET: PowerQuality
        public async Task<IActionResult> Index()
        {
            return View(await _context.PowerQuality.ToListAsync());
        }

        // GET: PowerQuality/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var powerQuality = await _context.PowerQuality
                .FirstOrDefaultAsync(m => m.ID == id);
            if (powerQuality == null)
            {
                return NotFound();
            }

            return View(powerQuality);
        }

        // GET: PowerQuality/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PowerQuality/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,MeterID,PowerFactor,Frequency,Voltage,InstantaneousPower,Timestamp")] PowerQuality powerQuality)
        {
            if (ModelState.IsValid)
            {
                _context.Add(powerQuality);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(powerQuality);
        }

        // GET: PowerQuality/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var powerQuality = await _context.PowerQuality.FindAsync(id);
            if (powerQuality == null)
            {
                return NotFound();
            }
            return View(powerQuality);
        }

        // POST: PowerQuality/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,MeterID,PowerFactor,Frequency,Voltage,InstantaneousPower,Timestamp")] PowerQuality powerQuality)
        {
            if (id != powerQuality.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(powerQuality);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PowerQualityExists(powerQuality.ID))
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
            return View(powerQuality);
        }

        // GET: PowerQuality/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var powerQuality = await _context.PowerQuality
                .FirstOrDefaultAsync(m => m.ID == id);
            if (powerQuality == null)
            {
                return NotFound();
            }

            return View(powerQuality);
        }

        // POST: PowerQuality/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var powerQuality = await _context.PowerQuality.FindAsync(id);
            if (powerQuality != null)
            {
                _context.PowerQuality.Remove(powerQuality);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PowerQualityExists(int id)
        {
            return _context.PowerQuality.Any(e => e.ID == id);
        }
    }
}
