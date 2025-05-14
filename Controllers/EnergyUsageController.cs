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
    public class EnergyUsageController : Controller
    {
        private readonly smartmetercmsContext _context;

        public EnergyUsageController(smartmetercmsContext context)
        {
            _context = context;
        }

        // GET: EnergyUsage
        public async Task<IActionResult> Index()
        {
            return View(await _context.EnergyUsage.ToListAsync());
        }

        // GET: EnergyUsage/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var energyUsage = await _context.EnergyUsage
                .FirstOrDefaultAsync(m => m.ID == id);
            if (energyUsage == null)
            {
                return NotFound();
            }

            return View(energyUsage);
        }

        // GET: EnergyUsage/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EnergyUsage/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,MeterID,Timestamp,EnergyUsed,Voltage,Current")] EnergyUsage energyUsage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(energyUsage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(energyUsage);
        }

        // GET: EnergyUsage/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var energyUsage = await _context.EnergyUsage.FindAsync(id);
            if (energyUsage == null)
            {
                return NotFound();
            }
            return View(energyUsage);
        }

        // POST: EnergyUsage/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,MeterID,Timestamp,EnergyUsed,Voltage,Current")] EnergyUsage energyUsage)
        {
            if (id != energyUsage.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(energyUsage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnergyUsageExists(energyUsage.ID))
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
            return View(energyUsage);
        }

        // GET: EnergyUsage/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var energyUsage = await _context.EnergyUsage
                .FirstOrDefaultAsync(m => m.ID == id);
            if (energyUsage == null)
            {
                return NotFound();
            }

            return View(energyUsage);
        }

        // POST: EnergyUsage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var energyUsage = await _context.EnergyUsage.FindAsync(id);
            if (energyUsage != null)
            {
                _context.EnergyUsage.Remove(energyUsage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnergyUsageExists(int id)
        {
            return _context.EnergyUsage.Any(e => e.ID == id);
        }
    }
}
