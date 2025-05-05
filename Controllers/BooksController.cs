using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotekaOnline.Data;
using BibliotekaOnline.Models;

namespace BibliotekaOnline.Controllers
{
    public class BooksController : Controller
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Books.ToListAsync());
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

        [AllowAnonymous]
public async Task<IActionResult> Index(string? search)
{
    var query = _context.Books.AsQueryable();

    if (!string.IsNullOrEmpty(search))
    {
        query = query.Where(b => b.Title.Contains(search) || b.Author.Contains(search));
    }

    var books = await query.ToListAsync();
    ViewBag.Search = search;
    return View(books);
}
    }
}