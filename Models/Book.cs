using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BibliotekosValdymoSistema.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        public string Genre { get; set; }

        [Display(Name = "Published Year")]
        public int PublishedYear { get; set; }

        public bool IsReserved { get; set; }

        public string? ReservedByUserId { get; set; }

        public virtual ICollection<LibraryWorker> LibraryWorkers { get; set; } = new List<LibraryWorker>();
    }
}
