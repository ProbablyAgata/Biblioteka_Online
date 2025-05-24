using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BibliotekaOnline.Data;
using BibliotekaOnline.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BibliotekaOnline.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // Just show the main page, no redirects
            // This allows both logged in and anonymous users to see the welcome page
            return View();
        }

        [Authorize]
        public IActionResult Catalog()
        {
            // Redirect to the Books controller Index action
            return RedirectToAction("Index", "Books");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminHome()
        {
            try
            {
                var recentBorrowings = await _context.Borrowings
                    .Include(b => b.Book)
                    .Include(b => b.User)
                    .OrderByDescending(b => b.BorrowDate)
                    .Take(5)
                    .ToListAsync();

                ViewBag.TotalBooks = await _context.Books.CountAsync();
                ViewBag.ActiveBorrowings = await _context.Borrowings.CountAsync(b => !b.Returned);
                ViewBag.RecentBorrowings = recentBorrowings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AdminHome action");
                ViewBag.TotalBooks = 0;
                ViewBag.ActiveBorrowings = 0;
                ViewBag.RecentBorrowings = new List<Borrowing>();
            }
            
            return View();
        }

        [Authorize]
        public async Task<IActionResult> UserHome()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                // Initialize with empty collections by default
                ViewBag.ActiveBorrowings = new List<Borrowing>();
                ViewBag.RecentlyReturned = new List<Borrowing>();
                
                if (!string.IsNullOrEmpty(userId))
                {
                    var activeBorrowings = await _context.Borrowings
                        .Include(b => b.Book)
                        .Where(b => b.UserId == userId && !b.Returned)
                        .ToListAsync();
                        
                    var recentlyReturned = await _context.Borrowings
                        .Include(b => b.Book)
                        .Where(b => b.UserId == userId && b.Returned)
                        .OrderByDescending(b => b.ReturnDate)
                        .Take(3)
                        .ToListAsync();
                        
                    ViewBag.ActiveBorrowings = activeBorrowings;
                    ViewBag.RecentlyReturned = recentlyReturned;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UserHome action");
                // Even on error, ensure ViewBag properties are not null
                ViewBag.ActiveBorrowings = ViewBag.ActiveBorrowings ?? new List<Borrowing>();
                ViewBag.RecentlyReturned = ViewBag.RecentlyReturned ?? new List<Borrowing>();
            }
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
} 
