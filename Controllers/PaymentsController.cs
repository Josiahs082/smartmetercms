using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smartmetercms.Data;
using smartmetercms.Models;

namespace smartmetercms.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly smartmetercmsContext _context;

        public PaymentsController(smartmetercmsContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var meterID = HttpContext.Session.GetString("MeterID");
            if (meterID == null) return RedirectToAction("Login", "Home");
            var bills = await _context.Bill.Where(b => b.MeterID == meterID && !b.PaidStatus).ToListAsync();
            return View(bills);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var payment = await _context.Payments.Include(p => p.Bill).FirstOrDefaultAsync(m => m.ID == id);
            if (payment == null) return NotFound();
            return View(payment);
        }

        public async Task<IActionResult> Pay(int? id)
        {
            if (id == null) return NotFound();
            var bill = await _context.Bill.FindAsync(id);
            if (bill == null) return NotFound("Bill not found.");
            if (bill.PaidStatus) return BadRequest("Bill is already paid.");
            var meterID = HttpContext.Session.GetString("MeterID");
            if (bill.MeterID != meterID) return Unauthorized("You are not authorized to pay this bill.");
            ViewData["PaymentMethods"] = new[] { "Credit Card", "Bank Transfer", "PayPal" };
            return View(bill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(int id, decimal paymentAmount, string paymentMethod)
        {
            var bill = await _context.Bill.FindAsync(id);
            if (bill == null) return NotFound("Bill not found.");
            if (bill.PaidStatus) return BadRequest("Bill is already paid.");
            var meterID = HttpContext.Session.GetString("MeterID");
            if (bill.MeterID != meterID) return Unauthorized("You are not authorized to pay this bill.");

            if (string.IsNullOrEmpty(paymentMethod))
            {
                ViewData["PaymentMethods"] = new[] { "Credit Card", "Bank Transfer", "PayPal" };
                ModelState.AddModelError("PaymentMethod", "Please select a payment method.");
                return View(bill);
            }

            if (paymentAmount <= 0 || paymentAmount > bill.AmountDue)
            {
                ViewData["PaymentMethods"] = new[] { "Credit Card", "Bank Transfer", "PayPal" };
                ModelState.AddModelError("PaymentAmount", "Payment amount must be between 0.01 and the amount due.");
                return View(bill);
            }

            var payment = new Payments
            {
                BillID = bill.ID,
                AmountPaid = Math.Round(paymentAmount, 2),
                PaymentDate = DateTime.Now,
                PaymentMethod = paymentMethod
            };

            bill.AmountDue = Math.Round(bill.AmountDue - paymentAmount, 2);
            bill.PaidStatus = bill.AmountDue <= 0.01m; // Tolerance for small residuals

            if (bill.PaidStatus)
            {
                bill.PaymentDate = payment.PaymentDate;
            }

            _context.Payments.Add(payment);
            _context.Bill.Update(bill);
            await _context.SaveChangesAsync();

            return RedirectToAction("CustomerDashboard", "Home");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,BillID,AmountPaid,PaymentDate,PaymentMethod")] Payments payments)
        {
            if (ModelState.IsValid)
            {
                payments.AmountPaid = Math.Round(payments.AmountPaid, 2);
                _context.Add(payments);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(payments);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var payments = await _context.Payments.FindAsync(id);
            if (payments == null) return NotFound();
            return View(payments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,BillID,AmountPaid,PaymentDate,PaymentMethod")] Payments payments)
        {
            if (id != payments.ID) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    payments.AmountPaid = Math.Round(payments.AmountPaid, 2);
                    _context.Update(payments);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentsExists(payments.ID)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(payments);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var payments = await _context.Payments.Include(p => p.Bill).FirstOrDefaultAsync(m => m.ID == id);
            if (payments == null) return NotFound();
            return View(payments);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payments = await _context.Payments.FindAsync(id);
            if (payments != null) _context.Payments.Remove(payments);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentsExists(int id)
        {
            return _context.Payments.Any(e => e.ID == id);
        }
    }
}