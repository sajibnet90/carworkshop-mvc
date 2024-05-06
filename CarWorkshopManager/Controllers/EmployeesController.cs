using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarWorkshopManager.Models;
using CarWorkshopManager.Data;

//filename: EmployeesController.cs
namespace CarWorkshopManager.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Employees
        // This method retrieves all employees and displays them.
        public async Task<IActionResult> Index()
        {
            return View(await _context.Employees.ToListAsync());
        }

        //-------------------------------------Create Employee-----------------
        // GET: Employees/Create
        // Method to display the Create Employee form.
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // Method to create a new employee.
        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            return View(employee);
        }

        //-----------------------------------Edit employee------------------------------
        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,HourlyRate")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(employee);
        }


        //_------------------------------Delete Employee------------------------------------------
        // GET: Employees/Delete/5
        // Method to confirm the deletion of a specific employee.
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Method to check if an employee exists.
        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
