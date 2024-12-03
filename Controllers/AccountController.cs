using BibliotekosValdymoSistema.Data;
using BibliotekosValdymoSistema.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BibliotekosValdymoSistema.Controllers
{
    public class AccountController : Controller
    {
        private readonly LibraryContext _libraryContext;

        public AccountController(LibraryContext context)
        {
            _libraryContext = context;
        }

        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _libraryContext.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View();
            }

            // If the user is a worker, update the LastLogin date for the LibraryWorker
            if (user.IsWorker)
            {
                var worker = await _libraryContext.LibraryWorkers
                    .FirstOrDefaultAsync(lw => lw.UserId == user.Id);

                if (worker != null)
                {
                    worker.LastLogin = DateTime.Now; // Update the LastLogin date
                    await _libraryContext.SaveChangesAsync(); // Save changes
                }
            }

            // Store user ID in session
            HttpContext.Session.SetString("UserId", user.Id.ToString());

            return RedirectToAction("Index", "Books");
        }


        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string password, string firstName, string lastName, bool isWorker)
        {
            // Check if the user already exists
            var existingUser = await _libraryContext.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (existingUser != null)
            {
                ModelState.AddModelError("", "Username already exists.");
                return View("Register"); // Show the register view again with an error
            }

            // Create the user and assign IsWorker correctly
            var user = new User
            {
                Username = username,
                Password = password,
                FirstName = firstName, 
                LastName = lastName,   
                IsWorker = isWorker    
            };

            // Add the user to the database
            _libraryContext.Users.Add(user);
            await _libraryContext.SaveChangesAsync();

            // If the user is a library worker, create and save the LibraryWorker record
            if (isWorker)
            {
                var libraryWorker = new LibraryWorker
                {
                    UserId = user.Id, 
                    DateOfBirth = DateTime.Now, 
                    LastLogin = DateTime.Now,
                    Salary = 2000 
                };

                _libraryContext.LibraryWorkers.Add(libraryWorker);
                await _libraryContext.SaveChangesAsync();
            }

            // Auto-login after registration
            HttpContext.Session.SetString("UserId", user.Id.ToString());

            return RedirectToAction("Index", "Books");
        }



        [HttpPost]
        public IActionResult Logout()
        {
            // Clear session on logout
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("UserName");

            return RedirectToAction("Index", "Books");
        }

        public async Task<IActionResult> ShowAllUsers()
        {
            // Fetch all users' names from the database
            var users = await _libraryContext.Users
                .Select(u => u.Username)
                .ToListAsync();

            ViewBag.Users = users;
            return View();
        }

    }

}