using System.ComponentModel.DataAnnotations;

namespace T2305M_API.Entities
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }

        public string Name { get; set; }

        // Navigation property for the many-to-many relationship
        public virtual ICollection<BookTag>? BookTags { get; set; }
    }
}
