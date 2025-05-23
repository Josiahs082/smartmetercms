using System;
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
        private const decimal RatePerKWh = 0.15m;

        public BillController(smartmetercmsContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Bill.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var bill = await _context.Bill.Include(b => b.Payments).FirstOrDefaultAsync(m => m.ID == id);
            if (bill == null) return NotFound();
            return View(bill);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,MeterID,BillingPeriodStart,BillingPeriodEnd,TotalEnergyUsed,AmountDue,PaidStatus,PaymentDate")] Bill bill)
        {
            if (ModelState.IsValid)
            {
                bill.AmountDue = Math.Round(bill.AmountDue, 2);
                _context.Add(bill);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bill);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var bill = await _context.Bill.FindAsync(id);
            if (bill == null) return NotFound();
            return View(bill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,MeterID,BillingPeriodStart,BillingPeriodEnd,TotalEnergyUsed,AmountDue,PaidStatus,PaymentDate")] Bill bill)
        {
            if (id != bill.ID) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    bill.AmountDue = Math.Round(bill.AmountDue, 2);
                    _context.Update(bill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillExists(bill.ID)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bill);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var bill = await _context.Bill.FirstOrDefaultAsync(m => m.ID == id);
            if (bill == null) return NotFound();
            return View(bill);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bill = await _context.Bill.FindAsync(id);
            if (bill != null) _context.Bill.Remove(bill);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateBills(DateTime startDate, DateTime endDate)
        {
            var users = await _context.User.ToListAsync();
            foreach (var user in users)
            {
                if (string.IsNullOrEmpty(user.MeterID)) continue;

                var energyUsage = await _context.IntervalEnergyUsage
                    .Where(ieu => ieu.MeterID == user.MeterID && ieu.Timestamp >= startDate && ieu.Timestamp <= endDate)
                    .SumAsync(ieu => ieu.EnergyUsed);

                if (energyUsage > 0)
                {
                    var bill = new Bill
                    {
                        MeterID = user.MeterID,
                        BillingPeriodStart = startDate,
                        BillingPeriodEnd = endDate,
                        TotalEnergyUsed = (float)energyUsage,
                        AmountDue = Math.Round((decimal)energyUsage * RatePerKWh, 2),
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