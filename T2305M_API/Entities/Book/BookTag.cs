using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace T2305M_API.Entities
{
    public class BookTag
    {
        public int BookId { get; set; }

        [ForeignKey("BookId")]
        public virtual Book? Book { get; set; }

        public int TagId { get; set; }

        [ForeignKey("TagId")]
        public virtual Tag? Tag { get; set; }
    }

}
