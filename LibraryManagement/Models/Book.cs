using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string Link { get; set; }
        public string? PdfPath { get; set; }  // Mark as optional
        public string? ImagePath { get; set; }  // Mark as optional
        public int Rating { get; set; }
    }


}

