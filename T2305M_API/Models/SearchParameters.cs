namespace T2305M_API.Models
{
    public class SearchParameters
    {
        const int maxPageSize = 50;
        public int Page { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value > maxPageSize ? maxPageSize : value; }
        }
        public string? Type { get; set; }
        public string? Country { get; set; }
        public string? Continent { get; set; }
        public string? SearchTerm { get; set; }  // Added search term for broader searching

        // New properties for sorting
        public string? SortColumn { get; set; } = "CreatedAt"; // Column name to sort by
        public string? SortOrder { get; set; } = "asc";  // "asc" or "desc" for sorting order
    }
}