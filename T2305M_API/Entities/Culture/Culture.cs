using System.ComponentModel.DataAnnotations;

namespace T2305M_API.Entities
{
    public class Culture
    {
        [Key]
        public int CultureId { get; set; }
        public string Title { get; set; }
        public string ThumbnailImage { get; set; }
        public string Description { get; set; }
        public string Period { get; set; }
        public string? Continent { get; set; }
        public string? Country { get; set; }
        public string CreatedAt { get; set; } = DateTime.UtcNow.ToString();
        public bool IsActive { get; set; } = true;
        public string? FileName { get; set; }
        public int CreatorId { get; set; }
        public virtual Creator? Creator { get; set; }  // for include or eagerly load
    }
}
