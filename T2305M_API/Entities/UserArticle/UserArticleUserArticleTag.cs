using System.ComponentModel.DataAnnotations.Schema;

namespace T2305M_API.Entities
{
    public class UserArticleUserArticleTag
    {
        public int UserArticleId { get; set; }
        [ForeignKey("UserArticleId")]
        public virtual UserArticle? UserArticle { get; set; }
        public int UserArticleTagId { get; set; }
        [ForeignKey("UserArticleTagId")]
        public virtual UserArticleTag? UserArticleTag { get; set; }
    }

}
