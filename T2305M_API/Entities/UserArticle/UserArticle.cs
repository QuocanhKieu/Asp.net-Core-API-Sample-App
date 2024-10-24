using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace T2305M_API.Entities
{
    public class UserArticle
    {
        [Key]
        public int UserArticleId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "NVARCHAR(MAX)")] // or "VARCHAR(MAX)" if you prefer
        public string Content { get; set; }
        public string ThumbnailImage { get; set; } = "/uploads/images/userArticleThumbnails/default-article-thumbnail.jpg";
        public bool IsPromoted { get; set; }
        public int UserId { get; set; }
        public virtual User? User { get; set; }  // for include or eagerly load
        public int? LikeCount { get; set; } = 0;
        public int? DislikeCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [Column(TypeName = "VARCHAR(50)")] // Adjust length as needed
        public string Status { get; set; } = "ACTIVE"; // Default value
        public virtual ICollection<UserArticleUserArticleTag>? userArticleUserArticleTags { get; set; }
        //public ICollection<UserArticleComment>? UserArticleComments { get; set; }
    }
}
