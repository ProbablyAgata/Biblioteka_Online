using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotekaOnline.Data;
using BibliotekaOnline.Models;
using System.Security.Claims;

namespace BibliotekaOnline.Controllers
{
    [Authorize]
    public class BorrowingsController : Controller
    {
        private readonly AppDbContext _context;

        public BorrowingsController(AppDbContext context)
        {
            _context = context;
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
                .ToListAsync();

            return View("AdminPanel", borrowings);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Users = _context.Users.ToList();
            ViewBag.Books = _context.Books.ToList();
            return View();
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

            borrowing.BorrowDate = DateTime.Now;
            _context.Borrowings.Add(borrowing);
            await _context.SaveChangesAsync();
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
                // If no ID is specified, show a list of active borrowings
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
            var borrowing = await _context.Borrowings.FindAsync(id);
            
            if (borrowing == null)
            {
                return NotFound();
            }
            
            borrowing.Returned = true;
            borrowing.ReturnDate = DateTime.Now;
            
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(AdminPanel));
        }
    }
}