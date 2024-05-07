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
            // Initialize a new ViewModel
            var viewModel = new CreateTicketViewModel();
 
            // Retrieve the list of employees from the database
            var employees = _context.Employees.ToList();
            ViewBag.Employees = new SelectList(employees, "Id", "Name");
            return View(viewModel);
        }


        // POST: Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTicketViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var ticket = new Ticket
                {
                    Brand = viewModel.Brand,
                    Model = viewModel.Model,
                    RegistrationId = viewModel.RegistrationId,
                    Description = viewModel.Description,
                    EmployeeId = viewModel.EmployeeId,
                    RepairSchedule = viewModel.RepairSchedule
                };

                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Reload the ViewModel's select list if the model state is not valid
            var employees = _context.Employees.ToList();
            ViewBag.Employees = new SelectList(employees, "Id", "Name", viewModel.EmployeeId);

            return View(viewModel);
        }



        //-----------------------------------------------------------------------------

        //---------------------------Adding Parts----------------------------------------------------
        // GET: Tickets/AddPart/5
        // GET: Tickets/AddPart/5
        public async Task<IActionResult> AddPart(int? ticketId)
        {
            if (ticketId == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                                       .Include(t => t.Parts) // Include the Parts related to this Ticket
                                       .FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null)
            {
                return NotFound();
            }

            ViewBag.Parts = ticket.Parts;  // Pass the list of parts to the view
            var part = new Part { TicketId = ticketId.Value };
            return View(part);
        }



        //---------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPart([Bind("Name, Description, Amount, UnitPrice, TicketId")] Part part)
        {
            if (ModelState.IsValid)
            {
                _context.Parts.Add(part);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Part added successfully.";
                //return RedirectToAction("Details", "Tickets", new { id = part.TicketId });
                return RedirectToAction("AddPart", new { ticketId = part.TicketId });

            }
            return View(part);
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

            // If the model state is not valid
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
                .Include(t => t.Employee)  // Make sure to include Employee
                .Include(t => t.Parts)
                .SingleOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            if (ticket.Employee == null)
            {
                return NotFound("Employee details are not available.");
            }

            // Calculate the total cost for parts
            var partsTotal = ticket.Parts.Sum(p => p.Amount * p.UnitPrice);

            // Calculate the total labor cost
            //var laborTotal = ticket.Hours * 10; // Assuming 10 euro hourly rate
            var laborTotal = ticket.Hours * ticket.Employee.HourlyRate;  // Calculate using the employee's hourly rate


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

            var ticket = await _context.Tickets
                                       .Include(t => t.Employee) 
                                       .AsNoTracking() // Recommended to avoid tracking issues
                                       .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            var viewModel = new TicketViewModel
            {
                Id = ticket.Id,
                Brand = ticket.Brand,
                Model = ticket.Model,
                RegistrationId = ticket.RegistrationId,
                Description = ticket.Description,
                EmployeeId = ticket.EmployeeId,
                RepairSchedule = ticket.RepairSchedule
            };

            var employees = _context.Employees.ToList();
            ViewBag.Employees = new SelectList(employees, "Id", "Name", ticket.EmployeeId);

            return View(viewModel);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TicketViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var ticket = await _context.Tickets.FindAsync(id);
                if (ticket == null)
                {
                    return NotFound();
                }

                ticket.Brand = viewModel.Brand;
                ticket.Model = viewModel.Model;
                ticket.RegistrationId = viewModel.RegistrationId;
                ticket.Description = viewModel.Description;
                ticket.EmployeeId = viewModel.EmployeeId;
                ticket.RepairSchedule = viewModel.RepairSchedule;

                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
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
            }

            var employees = _context.Employees.ToList();
            ViewBag.Employees = new SelectList(employees, "Id", "Name", viewModel.EmployeeId);
            return View(viewModel);
        }

        //----------------------------------------------------------------------
        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }

    }
}












//---------------------extra Create without ViewModel -----------------

//---------------------Create Tickets---------------------------------
//// GET: Tickets/Create
//public IActionResult Create()
//{
//    // Retrieve the list of employees from the database
//    var employees = _context.Employees.ToList();
//    // Pass the list of employees to the view
//    ViewBag.Employees = new SelectList(employees, "Id", "Name");
//    return View();
//}


//// POST: Tickets/Create
//[HttpPost]
//[ValidateAntiForgeryToken]
//public async Task<IActionResult> Create([Bind("Id,Brand,Model,RegistrationId,Description,EmployeeId,RepairSchedule")] Ticket ticket)
//{
//    if (ModelState.IsValid)
//    {
//        _context.Add(ticket);
//        await _context.SaveChangesAsync();
//        return RedirectToAction(nameof(Index));
//    }

//    // If model state is not valid, retrieve the lists of employees and repair schedules from the database again
//    var employees = _context.Employees.ToList();
//    // Pass the list of employees to the view
//    ViewBag.Employees = new SelectList(employees, "Id", "Name");

//    return View(ticket);
//}

//---------------------------------------------------------------------------
//---------------------extra EDIT without ViewModel -----------------
//// GET: Tickets/Edit/5
//public async Task<IActionResult> Edit(int? id)
//{
//    if (id == null)
//    {
//        return NotFound();
//    }

//    var ticket = await _context.Tickets.FindAsync(id);
//    if (ticket == null)
//    {
//        return NotFound();
//    }
//    // Retrieve the list of employees from the database
//    var employees = _context.Employees.ToList();
//    ViewBag.Employees = new SelectList(employees, "Id", "Name", ticket.EmployeeId);  // Set the current employee as the default

//    return View(ticket);
//}

//// POST: Tickets/Edit/5
//[HttpPost]
//[ValidateAntiForgeryToken]
//public async Task<IActionResult> Edit(int id, [Bind("Id,Brand,Model,RegistrationId,Description,EmployeeId,RepairSchedule")] Ticket ticket)
//{
//    if (id != ticket.Id)
//    {
//        return NotFound();
//    }

//    if (ModelState.IsValid)
//    {
//        try
//        {
//            _context.Update(ticket);
//            await _context.SaveChangesAsync();
//        }
//        catch (DbUpdateConcurrencyException)
//        {
//            if (!TicketExists(ticket.Id))
//            {
//                return NotFound();
//            }
//            else
//            {
//                throw;
//            }
//        }
//        return RedirectToAction(nameof(Index));
//    }
//    // Reload employees in case of an error
//    var employees = _context.Employees.ToList();
//    ViewBag.Employees = new SelectList(employees, "Id", "Name", ticket.EmployeeId);
//    return View(ticket);
//}

//---------------------extra-----------------