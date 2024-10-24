using System.ComponentModel.DataAnnotations;

namespace T2305M_API.Entities
{
    public class UserArticleTag
    {
        [Key]
        public int UserArticleTagId { get; set; }

        public string Name { get; set; }

        // Navigation property for the many-to-many relationship
        public virtual ICollection<UserArticleUserArticleTag>? userArticleUserArticleTags { get; set; }
    }
}
