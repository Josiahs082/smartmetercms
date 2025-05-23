using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smartmetercms.Data;
using smartmetercms.Models;

namespace smartmetercms.Controllers
{
    public class BillController : Controller
    {
        private readonly smartmetercmsContext _context;
        private const decimal RatePerKWh = 0.15m; // $0.15 per kWh

        public BillController(smartmetercmsContext context)
        {
            _context = context;
        }

        // GET: Bill
        public async Task<IActionResult> Index()
        {
            return View(await _context.Bill.ToListAsync());
        }

        // GET: Bill/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bill
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // GET: Bill/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bill/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,MeterID,BillingPeriodStart,BillingPeriodEnd,TotalEnergyUsed,AmountDue,PaidStatus,PaymentDate")] Bill bill)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bill);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bill);
        }

        // GET: Bill/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bill.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }
            return View(bill);
        }

        // POST: Bill/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,MeterID,BillingPeriodStart,BillingPeriodEnd,TotalEnergyUsed,AmountDue,PaidStatus,PaymentDate")] Bill bill)
        {
            if (id != bill.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillExists(bill.ID))
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
            return View(bill);
        }

        // GET: Bill/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bill
                .FirstOrDefaultAsync(m => m.ID == id);
            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // POST: Bill/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bill = await _context.Bill.FindAsync(id);
            if (bill != null)
            {
                _context.Bill.Remove(bill);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Bill/GenerateBills
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateBills(DateTime startDate, DateTime endDate)
        {
            var users = await _context.User.ToListAsync();
            foreach (var user in users)
            {
                var energyUsage = await _context.IntervalEnergyUsage
                    .Where(ieu => ieu.MeterID == user!.MeterID &&
                                 ieu.Timestamp >= startDate &&
                                 ieu.Timestamp <= endDate)
                    .SumAsync(ieu => ieu.EnergyUsed);

                if (energyUsage > 0)
                {
                    var bill = new Bill
                    {
                        MeterID = user!.MeterID,
                        BillingPeriodStart = startDate,
                        BillingPeriodEnd = endDate,
                        TotalEnergyUsed = energyUsage,
                        AmountDue = (decimal)energyUsage * RatePerKWh,
                        PaidStatus = false,
                        PaymentDate = null
                    };
                    _context.Bill.Add(bill);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BillExists(int id)
        {
            return _context.Bill.Any(e => e.ID == id);
        }
    }
}