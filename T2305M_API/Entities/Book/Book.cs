using System.ComponentModel.DataAnnotations;

namespace T2305M_API.Entities
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ThumbnailImage { get; set; }
        public bool IsRecommended { get; set; }
        public decimal Price { get; set; }
        public int CopiesSold { get; set; } = 0;
        public DateTime ReleaseDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public int CreatorId { get; set; }
        public virtual Creator? Creator { get; set; }  // for include or eagerly load
        public virtual ICollection<BookTag>? BookTags { get; set; } = new List<BookTag>();

        // New Description column
        public string? Description { get; set; } // Optional, so it allows nulls
    }

}
