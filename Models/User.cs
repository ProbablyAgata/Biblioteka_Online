using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BibliotekaOnline.Models
{
    public class User : IdentityUser
    {
        public ICollection<Borrowing>? Borrowings { get; set; }
    }
}