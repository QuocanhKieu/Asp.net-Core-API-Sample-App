
namespace T2305M_API.DTO.Exhibition
{
    public class ExhibitionArticleDto
    {
        public int ExhibitionArticleId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string ThumbnailImage { get; set; }
        public string CreatorName { get; set; }
        public int CreatorId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Location { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Organizer { get; set; }
        public string Type { get; } = "ExhibitionArticles";

    }

    public class ExhibitionArticleDetailDto
    {
        public int ExhibitionArticleId { get; set; }  // Primary Key
        public string Title { get; set; }  // Title of the article
        public string Content { get; set; }  // Content of the article
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Organizer { get; set; }
        public string? Location { get; set; }
        public string ThumbnailImage { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Continent { get; set; }  // E.g., "Africa", "Asia", "Europe", etc.
        public string? Country { get; set; }  // E.g., "USA", "China", etc.
        public int? CreatorId { get; set; }  // Foreign Key to Creator
        public string CreatorName { get; set; }
        public ICollection<String>? ExhibitionArticleImageURLs { get; set; }
    }

    public class ExhibitionQueryParameters
    {
        const int maxPageSize = 50;
        public int Page { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value > maxPageSize ? maxPageSize : value; }
        }

        public string? Title { get; set; }
        public string? Country { get; set; }
        public string? Continent { get; set; }
        public string? SearchTerm { get; set; }  // Added search term for broader searching

        // New properties for sorting
        public string? SortColumn { get; set; }  // Column name to sort by
        public string? SortOrder { get; set; } = "asc";  // "asc" or "desc" for sorting order

    }
}
