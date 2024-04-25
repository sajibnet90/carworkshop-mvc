using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarWorkshopManager.Models;
using CarWorkshopManager.Data;


namespace CarWorkshopManager.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            var tickets = _context.Tickets.Include(t => t.Parts);
            return View(await tickets.ToListAsync());
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Brand,Model,RegistrationId,Description")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }


        //-------------------------------------------------
        // GET: Tickets/AddPart/5
        public async Task<IActionResult> AddPart(int? ticketId)
        {
            if (ticketId == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.Include(t => t.Parts).FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null)
            {
                return NotFound();
            }

            var viewModel = new AddPartViewModel
            {
                TicketId = ticketId.Value,
                Parts = ticket.Parts.ToList(),
                Part = new Part { TicketId = ticketId.Value }
            };

            return View(viewModel);
        }

        // POST: Tickets/AddPart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPart([Bind("Name,Description,Amount,UnitPrice,TicketId")] Part part)
        {
            if (ModelState.IsValid)
            {
                _context.Parts.Add(part);
                await _context.SaveChangesAsync();

                // Fetch the updated list of parts for the ticket
                var parts = await _context.Parts
                                          .Where(p => p.TicketId == part.TicketId)
                                          .ToListAsync();

                // Prepare the ViewModel to pass to the view
                var viewModel = new AddPartViewModel
                {
                    TicketId = part.TicketId,
                    Parts = parts,
                    Part = new Part { TicketId = part.TicketId }  // Reset the part input form
                };

                return View(viewModel);
            }

            // If model state is not valid, just return the view with the current part data
            return View(new AddPartViewModel
            {
                TicketId = part.TicketId,
                //Parts = new List<Part>(), // Ensure Parts is initialized
                Part = part
            });
        }

        //-------------------------------------------------------
        // GET: Tickets/AddHours/5
        public IActionResult AddHours(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = _context.Tickets.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }

            // Pass the ticket to the view for displaying
            return View(ticket);
        }

        // POST: Tickets/AddHours
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddHours(int id, int hoursToAdd)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                ticket.Hours += hoursToAdd;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(ticket);
        }

        // GET: Tickets/ViewCost/5
        public async Task<IActionResult> ViewCost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Parts)
                .SingleOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            // Calculate the total cost for parts
            var partsTotal = ticket.Parts.Sum(p => p.Amount * p.UnitPrice);

            // Calculate the total labor cost
            var laborTotal = ticket.Hours * 10; // Assuming 10 is your hourly rate

            // Calculate the grand total
            var grandTotal = partsTotal + laborTotal;

            // Create a ViewModel to pass this information to the view
            var viewModel = new TicketCostViewModel
            {
                Ticket = ticket,
                PartsTotal = partsTotal,
                LaborTotal = laborTotal,
                GrandTotal = grandTotal
            };

            return View(viewModel);
        }

        //------------------------------------------
        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Parts) // If the ticket has related parts, include them for deletion
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            // Remove related parts if there are any
            foreach (var part in ticket.Parts.ToList())
            {
                _context.Parts.Remove(part);
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //-----------------------
        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Brand,Model,RegistrationId,Description")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
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
            return View(ticket);
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }



    }
}
