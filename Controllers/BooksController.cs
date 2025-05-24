using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotekaOnline.Data;
using BibliotekaOnline.Models;
using BibliotekaOnline.Services;
using BibliotekaOnline.Models.GoogleBooks;

namespace BibliotekaOnline.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly AppDbContext _context;
        private readonly GoogleBooksService _googleBooksService;

        public BooksController(AppDbContext context, GoogleBooksService googleBooksService)
        {
            _context = context;
            _googleBooksService = googleBooksService;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b => b.Title.Contains(search) || 
                                        b.Author.Contains(search) || 
                                        b.ISBN.Contains(search));
            }

            var books = await query.ToListAsync();
            ViewBag.Search = search;
            return View(books);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Book book)
        {
            if (!ModelState.IsValid) return View(book);

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GoogleBooksSearch(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View(new GoogleBookApiResponse());
            }

            var results = await _googleBooksService.SearchBooksAsync(query);
            return View(results);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ImportFromGoogleBooks(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Book ID is required");
            }

            // Check if book already exists
            var existingBook = await _context.Books.FirstOrDefaultAsync(b => b.GoogleBookId == id);
            if (existingBook != null)
            {
                TempData["Error"] = "This book is already in the library";
                return RedirectToAction(nameof(Index));
            }

            var googleBook = await _googleBooksService.GetBookByIdAsync(id);
            if (googleBook == null)
            {
                return NotFound();
            }

            var book = new Book
            {
                Title = googleBook.VolumeInfo.Title,
                Author = string.Join(", ", googleBook.VolumeInfo.Authors),
                Description = googleBook.VolumeInfo.Description,
                TotalCopies = 1, // Default value
                GoogleBookId = googleBook.Id,
                ISBN = googleBook.VolumeInfo.IndustryIdentifiers?.FirstOrDefault()?.Identifier ?? string.Empty,
                Publisher = googleBook.VolumeInfo.Publisher,
                PublishedDate = googleBook.VolumeInfo.PublishedDate,
                PageCount = googleBook.VolumeInfo.PageCount,
                Categories = string.Join(", ", googleBook.VolumeInfo.Categories ?? new List<string>()),
                Language = googleBook.VolumeInfo.Language,
                ThumbnailUrl = googleBook.VolumeInfo.ImageLinks?.Thumbnail ?? string.Empty,
                PreviewLink = googleBook.AccessInfo?.WebReaderLink ?? string.Empty
            };

            return View("ImportFromGoogleBooks", book);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmImport(Book book)
        {
            if (!ModelState.IsValid)
            {
                return View("ImportFromGoogleBooks", book);
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Book successfully imported from Google Books";
            return RedirectToAction(nameof(Index));
        }
    }
}