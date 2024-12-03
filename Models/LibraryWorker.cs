using System;
using System.Collections.Generic;

namespace BibliotekosValdymoSistema.Models
{
    public class LibraryWorker
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime LastLogin { get; set; }
        public decimal Salary { get; set; }

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();

        public virtual User User { get; set; }

        public string ?WorkHours { get; set; }

    }
}
