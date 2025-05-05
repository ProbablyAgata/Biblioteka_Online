using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotekaOnline.Models
{
    public class Borrowing
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int BookId { get; set; }

        [Required]
        public DateTime BorrowDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        [Required]
        public bool Returned { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [ForeignKey("BookId")]
        public Book? Book { get; set; }
    }
}