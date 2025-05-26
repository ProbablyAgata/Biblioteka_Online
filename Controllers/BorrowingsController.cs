using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotekaOnline.Data;
using BibliotekaOnline.Models;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace BibliotekaOnline.Controllers
{
    [Authorize]
    public class BorrowingsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BorrowingsController> _logger;

        public BorrowingsController(AppDbContext context, ILogger<BorrowingsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var borrowings = await _context.Borrowings
                .Include(b => b.Book)
                .Where(b => b.UserId == userId && !b.Returned)
                .ToListAsync();

            return View(borrowings);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminPanel()
        {
            var borrowings = await _context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.User)
                .OrderByDescending(b => b.BorrowDate)
                .Take(10)
                .ToListAsync();

            ViewBag.ActiveBorrowings = await _context.Borrowings.CountAsync(b => !b.Returned);
            ViewBag.TotalBooks = await _context.Books.CountAsync();

            return View("AdminPanel", borrowings);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Users = _context.Users.ToList();
            ViewBag.Books = _context.Books.ToList();
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Borrow(int? bookId)
        {
            if (bookId == null)
            {
                return RedirectToAction("Index", "Books");
            }

            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                return NotFound();
            }

            if (book.TotalCopies <= 0)
            {
                TempData["Error"] = "Przepraszamy, ale wszystkie egzemplarze tej książki są obecnie wypożyczone.";
                return RedirectToAction("Details", "Books", new { id = bookId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingBorrowing = await _context.Borrowings
                .FirstOrDefaultAsync(b => b.UserId == userId && b.BookId == bookId && !b.Returned);

            if (existingBorrowing != null)
            {
                TempData["Error"] = "Już wypożyczyłeś tę książkę.";
                return RedirectToAction("Details", "Books", new { id = bookId });
            }

            ViewBag.Book = book;
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Borrow(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                _logger.LogWarning($"Book with ID {bookId} not found");
                TempData["Error"] = "Książka nie została znaleziona.";
                return RedirectToAction("Index", "Books");
            }

            _logger.LogInformation($"POST: Attempting to borrow book {book.Title} (ID: {book.Id}). Current copies: {book.TotalCopies}");

            if (book.TotalCopies <= 0)
            {
                _logger.LogWarning($"POST: Cannot borrow book {book.Title} - no copies available");
                TempData["Error"] = "Przepraszamy, ale wszystkie egzemplarze tej książki są obecnie wypożyczone.";
                return RedirectToAction("Details", "Books", new { id = bookId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var existingBorrowing = await _context.Borrowings
                .FirstOrDefaultAsync(b => b.UserId == userId && b.BookId == bookId && !b.Returned);
                
            if (existingBorrowing != null)
            {
                _logger.LogWarning($"POST: User {userId} already has an active borrowing for book {book.Title}");
                TempData["Error"] = "Już wypożyczyłeś tę książkę.";
                return RedirectToAction("Details", "Books", new { id = bookId });
            }

            var borrowing = new Borrowing
            {
                UserId = userId,
                BookId = bookId,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                Returned = false
            };

            book.TotalCopies--;
            _logger.LogInformation($"POST: Decreasing copies for book {book.Title}. New count: {book.TotalCopies}");
            
            _context.Borrowings.Add(borrowing);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = $"Pomyślnie wypożyczyłeś książkę '{book.Title}'. Termin zwrotu: {borrowing.DueDate:dd.MM.yyyy}";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Borrowing borrowing)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Users = _context.Users.ToList();
                ViewBag.Books = _context.Books.ToList();
                return View(borrowing);
            }

            var book = await _context.Books.FindAsync(borrowing.BookId);
            if (book == null)
            {
                TempData["Error"] = "Książka nie została znaleziona.";
                ViewBag.Users = _context.Users.ToList();
                ViewBag.Books = _context.Books.ToList();
                return View(borrowing);
            }

            if (book.TotalCopies <= 0)
            {
                TempData["Error"] = "Przepraszamy, ale wszystkie egzemplarze tej książki są obecnie wypożyczone.";
                ViewBag.Users = _context.Users.ToList();
                ViewBag.Books = _context.Books.ToList();
                return View(borrowing);
            }

            borrowing.BorrowDate = DateTime.Now;
            borrowing.DueDate = DateTime.Now.AddDays(14);
            borrowing.Returned = false;

            book.TotalCopies--;
            _context.Borrowings.Add(borrowing);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Pomyślnie utworzono wypożyczenie książki '{book.Title}'.";
            return RedirectToAction(nameof(AdminPanel));
        }

        [Authorize]
        public async Task<IActionResult> Archive()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var archived = await _context.Borrowings
                .Include(b => b.Book)
                .Where(b => b.UserId == userId && b.Returned)
                .OrderByDescending(b => b.ReturnDate)
                .ToListAsync();

            return View("Archive", archived);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkReturned(int? id)
        {
            if (id == null)
            {
                var activeBorrowings = await _context.Borrowings
                    .Include(b => b.Book)
                    .Include(b => b.User)
                    .Where(b => !b.Returned)
                    .ToListAsync();
                
                ViewBag.ActiveBorrowings = activeBorrowings;
                return View();
            }
            
            var borrowing = await _context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);
                
            if (borrowing == null)
            {
                return NotFound();
            }
            
            return View(borrowing);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkReturned(int id)
        {
            var borrowing = await _context.Borrowings
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.Id == id);
            
            if (borrowing == null)
            {
                return NotFound();
            }
            
            var returnDate = DateTime.Now;
            if (returnDate < borrowing.BorrowDate)
            {
                returnDate = borrowing.BorrowDate;
            }
            
            borrowing.Returned = true;
            borrowing.ReturnDate = returnDate;
            
            if (borrowing.Book != null)
            {
                borrowing.Book.TotalCopies++;
            }
            
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Książka została pomyślnie oznaczona jako zwrócona.";
            
            return RedirectToAction(nameof(AdminPanel));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var borrowing = await _context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);
                
            if (borrowing == null)
            {
                return NotFound();
            }
            
            ViewBag.Users = _context.Users.ToList();
            ViewBag.Books = _context.Books.ToList();
            
            return View(borrowing);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Borrowing borrowing)
        {
            if (id != borrowing.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Users = _context.Users.ToList();
                ViewBag.Books = _context.Books.ToList();
                return View(borrowing);
            }

            try
            {
                var existingBorrowing = await _context.Borrowings.FindAsync(id);
                if (existingBorrowing == null)
                {
                    return NotFound();
                }

                existingBorrowing.UserId = borrowing.UserId;
                existingBorrowing.BookId = borrowing.BookId;
                existingBorrowing.BorrowDate = borrowing.BorrowDate;
                existingBorrowing.DueDate = borrowing.DueDate;
                existingBorrowing.ReturnDate = borrowing.ReturnDate;
                existingBorrowing.Returned = borrowing.ReturnDate.HasValue;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Wypożyczenie zostało pomyślnie zaktualizowane.";
                return RedirectToAction(nameof(AdminPanel));
            }
            catch (Exception)
            {
                TempData["Error"] = "Wystąpił błąd podczas aktualizacji wypożyczenia.";
                ViewBag.Users = _context.Users.ToList();
                ViewBag.Books = _context.Books.ToList();
                return View(borrowing);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Return(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var borrowing = await _context.Borrowings
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
            
            if (borrowing == null)
            {
                TempData["Error"] = "Nie znaleziono wypożyczenia lub nie masz uprawnień do tej operacji.";
                return RedirectToAction(nameof(Index));
            }
            
            if (borrowing.ReturnDate.HasValue)
            {
                TempData["Error"] = "Ta książka została już zwrócona.";
                return RedirectToAction(nameof(Index));
            }
            
            var returnDate = DateTime.Now;
            if (returnDate < borrowing.BorrowDate)
            {
                returnDate = borrowing.BorrowDate;
            }
            
            borrowing.Returned = true;
            borrowing.ReturnDate = returnDate;
            
            await _context.SaveChangesAsync();
            
            TempData["Success"] = $"Książka '{borrowing.Book?.Title}' została pomyślnie zwrócona.";
            
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RequestExtension(int borrowingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var borrowing = await _context.Borrowings
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.Id == borrowingId && b.UserId == userId && !b.Returned);
            
            if (borrowing == null)
            {
                TempData["Error"] = "Nie znaleziono wypożyczenia lub nie masz uprawnień do tej operacji.";
                return RedirectToAction("UserHome", "Home");
            }

            borrowing.DueDate = borrowing.DueDate.AddDays(7);
            
            await _context.SaveChangesAsync();
            
            TempData["Success"] = $"Termin zwrotu książki '{borrowing.Book?.Title}' został przedłużony do {borrowing.DueDate:dd.MM.yyyy}.";
            
            return RedirectToAction("UserHome", "Home");
        }
    }
}