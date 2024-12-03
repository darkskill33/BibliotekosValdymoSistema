using BibliotekosValdymoSistema.Data;
using BibliotekosValdymoSistema.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotekosValdymoSistema.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryContext _libraryContext;

        public BooksController (LibraryContext context)
        {
            _libraryContext = context;
        }

        // Book indexing
        public async Task<IActionResult> Index(string searchString)
        {
            var books = from book in _libraryContext.Books select book;

            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.Title.Contains(searchString) || b.Author.Contains(searchString));
            }

            // Get the user ID and username from session
            var userId = HttpContext.Session.GetString("UserId");
            var userName = HttpContext.Session.GetString("UserName");

            // Safe assignment to ViewBag
            ViewBag.IsLoggedIn = !string.IsNullOrEmpty(userId); // true if user is logged in, otherwise false
            ViewBag.UserName = userName; // set the username if logged in, null if not

            return View(await books.ToListAsync());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var book = await _libraryContext.Books.FirstOrDefaultAsync(book => book.Id == id);

            if (book == null)
                return NotFound();

            // Pass the user ID from the session to the view
            ViewBag.UserId = HttpContext.Session.GetString("UserId");

            return View(book);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Genre,PublishedYear")] Book book)
        {
            if (ModelState.IsValid)
            {
                _libraryContext.Add(book);
                await _libraryContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }


        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _libraryContext.Books.FindAsync(id);
            if (book == null) return NotFound();

            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Genre,PublishedYear")] Book book)
        {
            if (id != book.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _libraryContext.Update(book);
                    await _libraryContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _libraryContext.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null) return NotFound();

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _libraryContext.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _libraryContext.Books.Remove(book);
            await _libraryContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Redirects to the Index view
        }


        private bool BookExists(int id)
        {
            return _libraryContext.Books.Any(e => e.Id == id);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(int id)
        {
            var book = await _libraryContext.Books.FindAsync(id);
            if (book == null || book.IsReserved)
            {
                return NotFound(); // If book not found or already reserved
            }

            // Assuming user is authenticated
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if not authenticated
            }

            book.IsReserved = true;
            book.ReservedByUserId = userId;
            _libraryContext.Update(book);
            await _libraryContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var book = await _libraryContext.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }



            book.IsReserved = false;
            book.ReservedByUserId = null;

            // Save changes to the database
            _libraryContext.Update(book);
            await _libraryContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        

    }
}
