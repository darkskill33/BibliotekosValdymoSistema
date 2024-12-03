using BibliotekosValdymoSistema.Data;
using BibliotekosValdymoSistema.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BibliotekosValdymoSistema.Controllers
{
    public class LibraryWorkersController : Controller
    {
        private readonly LibraryContext _context;

        public LibraryWorkersController(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch all library workers, including user info
            var workers = await _context.LibraryWorkers
                .Include(lw => lw.User)
                .Include(lw => lw.Books) 
                .ToListAsync();
            return View(workers);
        }

        // Method to add a new library worker
        public IActionResult Create()
        {
            // Populate a list of users for selection
            ViewBag.Users = new SelectList(_context.Users.ToList(), "Id", "Username");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LibraryWorker worker)
        {
            if (ModelState.IsValid)
            {
                _context.LibraryWorkers.Add(worker);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Repopulate ViewBag if model state is invalid
            ViewBag.Users = new SelectList(_context.Users.ToList(), "Id", "Username");
            return View(worker);
        }

        public async Task<IActionResult> AssignBooks(int id)
        {
            var worker = await _context.LibraryWorkers
                .Include(w => w.User)
                .Include(w => w.Books)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (worker == null) return NotFound();

            var availableBooks = await _context.Books.ToListAsync();
            ViewBag.Books = availableBooks;
            return View(worker);
        }


        // POST: Assign Books to a Worker
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignBooks(int id, List<int> bookIds)
        {
            // Retrieve the worker from the database, including their current books
            var worker = await _context.LibraryWorkers
                .Include(w => w.Books)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (worker == null) return NotFound();

            // Debugging: Log the received bookIds to ensure they are coming through
           /*if (bookIds != null)
            {
                Console.WriteLine($"Number of selected books: {bookIds.Count}");
                foreach (var bookId in bookIds)
                {
                    Console.WriteLine($"Selected Book ID: {bookId}");
                }
            }
            else
            {
                Console.WriteLine("No books selected.");
            }


*/
            // Clear current book assignments (if necessary)
            worker.Books.Clear();
           

            // Reassign selected books to the worker
            if (bookIds != null && bookIds.Count > 0)
            {
                var selectedBooks = await _context.Books
                    .Where(b => bookIds.Contains(b.Id))
                    .ToListAsync();

                // Assign the selected books to the worker
                foreach (var book in selectedBooks)
                {
                    worker.Books.Add(book);
                }
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect back to the Index view
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> ViewBooks(int id)
        {
            var worker = await _context.LibraryWorkers
                .Include(w => w.User)
                .Include(w => w.Books) // Make sure to include the Books
                .FirstOrDefaultAsync(w => w.Id == id);

            if (worker == null) return NotFound();

            return View(worker);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveBook(int workerId, int bookId)
        {
            var worker = await _context.LibraryWorkers
                .Include(w => w.Books) // Include books assigned to the worker
                .FirstOrDefaultAsync(w => w.Id == workerId);

            if (worker == null) return NotFound();

            // Find the book to remove from the worker's assigned books
            var bookToRemove = worker.Books.FirstOrDefault(b => b.Id == bookId);
            if (bookToRemove != null)
            {
                worker.Books.Remove(bookToRemove); 
                await _context.SaveChangesAsync(); 
            }

            // Redirect back to the view of assigned books after removing
            return RedirectToAction("ViewBooks", new { id = workerId });
        }

        public async Task<IActionResult> WorkCalendar(int id)
        {
            var worker = await _context.LibraryWorkers
                .Include(w => w.User)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (worker == null) return NotFound();

            // Check if work hours have already been set
            if (string.IsNullOrEmpty(worker.WorkHours))
            {
                // Generate random work calendar (assuming a 7-day week)
                var daysOfWeek = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
                var possibleWorkHours = new List<string>
        {
            "8:00 AM - 4:00 PM",
            "9:00 AM - 5:00 PM",
            "10:00 AM - 6:00 PM",
            "7:00 AM - 3:00 PM",
            "12:00 PM - 8:00 PM"
        };

                var random = new Random();

                // Randomly choose 3 workdays
                var workDays = daysOfWeek.OrderBy(x => random.Next()).Take(3).ToList();

                // Create a dictionary to hold work hours for selected workdays
                var workHoursSchedule = new Dictionary<string, string>();

                // Assign random work hours for the selected workdays
                foreach (var day in workDays)
                {
                    workHoursSchedule[day] = possibleWorkHours[random.Next(possibleWorkHours.Count)];
                }

                // Store the work hours in the worker model
                worker.WorkHours = JsonConvert.SerializeObject(workHoursSchedule); // Or any other serialization method you prefer

                // Update the database with the new work hours
                _context.LibraryWorkers.Update(worker);
                await _context.SaveChangesAsync();

                // Pass the work days and their respective work hours to the view
                ViewBag.WorkDays = workDays;
                ViewBag.WorkHoursSchedule = workHoursSchedule;
            }
            else
            {
                // Deserialize work hours if already set
                var workHoursSchedule = JsonConvert.DeserializeObject<Dictionary<string, string>>(worker.WorkHours);

                // Get the list of work days
                var workDays = workHoursSchedule.Keys.ToList();

                // Pass the worker and the existing work hours to the view
                ViewBag.WorkDays = workDays;
                ViewBag.WorkHoursSchedule = workHoursSchedule;
            }

            return View(worker);
        }

    }
}
