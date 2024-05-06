using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarWorkshopManager.Models;
using CarWorkshopManager.Data;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace CarWorkshopManager.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }
        //--------------------------------Index action method------------------------------------
        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            var tickets = _context.Tickets.Include(t => t.Parts);
            return View(await tickets.ToListAsync());
        }
        

        //---------------------Create Tickets---------------------------------
        // GET: Tickets/Create
        public IActionResult Create()
        {
            // Retrieve the list of employees from the database
            var employees = _context.Employees.ToList();
            // Pass the list of employees to the view
            ViewBag.Employees = new SelectList(employees, "Id", "Name");
            return View();
        }


        // POST: Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Brand,Model,RegistrationId,Description,EmployeeId,RepairSchedule")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If model state is not valid, retrieve the lists of employees and repair schedules from the database again
            var employees = _context.Employees.ToList();
            // Pass the list of employees to the view
            ViewBag.Employees = new SelectList(employees, "Id", "Name");

            return View(ticket);
        }


        //-----------------------------------------------------------------------------

        //---------------------------Parts----------------------------------------------------
        // GET: Tickets/AddPart/5
        public async Task<IActionResult> AddPart(int? ticketId)
        {
            if (ticketId == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                return NotFound();
            }

            var part = new Part { TicketId = ticketId.Value };  

            return View(part);
        }


        //---------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPart([Bind("PartId, Name, Description, Amount, UnitPrice, TicketId")] Part part)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                TempData["ErrorMessage"] = string.Join(", ", errors.Select(e => e.ErrorMessage));
                return View(part);
            }

            _context.Parts.Add(part);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Part added successfully.";
            return RedirectToAction("Details", "Tickets", new { id = part.TicketId });
        }


        //------------------------------Adding Hours-----------------------------------------
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
        public async Task<IActionResult> AddHours(int id, int hoursWorked)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                ticket.Hours += hoursWorked; 
                _context.Update(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If the model state is not valid, or you want to handle the form differently
            return View(ticket);
        }

        //----------------------------------------------View Cost------------------------------
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
            var laborTotal = ticket.Hours * 10; // Assuming 10 euro hourly rate

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

        //----------------------------Delete Ticket---------------------------------------
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


        //------------------------------Edit ticket----------------------------------------
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
            // Retrieve the list of employees from the database
            var employees = _context.Employees.ToList();
            ViewBag.Employees = new SelectList(employees, "Id", "Name", ticket.EmployeeId);  // Set the current employee as the default

            return View(ticket);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Brand,Model,RegistrationId,Description,EmployeeId,RepairSchedule")] Ticket ticket)
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
            // Reload employees in case of an error
            var employees = _context.Employees.ToList();
            ViewBag.Employees = new SelectList(employees, "Id", "Name", ticket.EmployeeId);
            return View(ticket);
        }


        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }


    }
}
