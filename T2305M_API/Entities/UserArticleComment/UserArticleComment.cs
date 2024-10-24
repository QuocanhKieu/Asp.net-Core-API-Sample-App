using System.ComponentModel.DataAnnotations;

namespace T2305M_API.Entities.UserArticleComment
{
    public class UserArticleComment
    {
        [Key]
        public int UserArticleCommentId { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; } // The user who made the comment
        public int UserArticleId { get; set; } // The article being commented on
        public int? ParentCommentId { get; set; } // The ID of the parent comment, if this is a reply
        public int? TopLevelCommentId { get; set; } // ID of the top-level comment being replied to
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int LikeCount { get; set; } = 0;

        // Navigation properties for relationships
        public virtual UserArticleComment? ParentComment { get; set; } // Reference to the parent comment
        public virtual UserArticleComment? TopLevelComment { get; set; } // Reference to the parent comment
        public virtual ICollection<UserArticleComment> Replies { get; set; } // Collection of replies to this comment
    }

}
