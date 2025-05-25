using System.ComponentModel.DataAnnotations;

namespace BibliotekaOnline.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Author { get; set; } = string.Empty;

        [StringLength(5000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Total copies must be at least 1")]
        public int TotalCopies { get; set; }

        // Google Books specific fields
        public string GoogleBookId { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string PublishedDate { get; set; } = string.Empty;
        public int PageCount { get; set; }
        public string Categories { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string PreviewLink { get; set; } = string.Empty;

        public ICollection<Borrowing>? Borrowings { get; set; }
    }
}