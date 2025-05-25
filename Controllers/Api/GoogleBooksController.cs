using Microsoft.AspNetCore.Mvc;
using BibliotekaOnline.Models.GoogleBooks;
using BibliotekaOnline.Services;
using BibliotekaOnline.Data;
using BibliotekaOnline.Models;

namespace BibliotekaOnline.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleBooksController : ControllerBase
    {
        private readonly GoogleBooksService _googleBooksService;
        private readonly AppDbContext _context;

        public GoogleBooksController(GoogleBooksService googleBooksService, AppDbContext context)
        {
            _googleBooksService = googleBooksService;
            _context = context;
        }

        [HttpGet("search")]
        public async Task<ActionResult<GoogleBookApiResponse>> Search([FromQuery] string query, [FromQuery] int maxResults = 10)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Parametr zapytania jest wymagany");
            }

            var result = await _googleBooksService.SearchBooksAsync(query, maxResults);
            return Ok(result);
        }

        [HttpGet("isbn/{isbn}")]
        public async Task<ActionResult<GoogleBookApiResponse>> SearchByIsbn(string isbn, [FromQuery] int maxResults = 10)
        {
            if (string.IsNullOrEmpty(isbn))
            {
                return BadRequest("ISBN jest wymagane");
            }

            var result = await _googleBooksService.SearchBooksByIsbnAsync(isbn, maxResults);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GoogleBookItem>> GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID jest wymagane");
            }

            var result = await _googleBooksService.GetBookByIdAsync(id);
            
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost("import/{id}")]
        public async Task<ActionResult<Book>> ImportBook(string id, [FromQuery] int totalCopies = 1)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID jest wymagane");
            }

            var existingBook = _context.Books.FirstOrDefault(b => b.GoogleBookId == id);
            if (existingBook != null)
            {
                return Conflict($"Książka z Google ID {id} już istnieje w bazie danych");
            }

            var googleBook = await _googleBooksService.GetBookByIdAsync(id);
            if (googleBook == null)
            {
                return NotFound($"Książka z Google ID {id} nie znaleziona");
            }

            var book = new Book
            {
                Title = googleBook.VolumeInfo.Title,
                Author = string.Join(", ", googleBook.VolumeInfo.Authors),
                Description = googleBook.VolumeInfo.Description,
                TotalCopies = totalCopies,
                GoogleBookId = googleBook.Id,
                ISBN = googleBook.VolumeInfo.IndustryIdentifiers.FirstOrDefault()?.Identifier ?? string.Empty,
                Publisher = googleBook.VolumeInfo.Publisher,
                PublishedDate = googleBook.VolumeInfo.PublishedDate,
                PageCount = googleBook.VolumeInfo.PageCount,
                Categories = string.Join(", ", googleBook.VolumeInfo.Categories),
                Language = googleBook.VolumeInfo.Language,
                ThumbnailUrl = googleBook.VolumeInfo.ImageLinks.Thumbnail,
                PreviewLink = googleBook.AccessInfo.WebReaderLink
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }
    }
} 