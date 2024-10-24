using System.ComponentModel.DataAnnotations;

namespace T2305M_API.DTO.Culture
{
    public class GetBasicCultureDTO
    {
        [Key]
        public int CultureId { get; set; }
        public string Title { get; set; }
        public string ThumbnailImage { get; set; }
        public string Description { get; set; }
        public string Type { get; } = "Culture";
        public string CreatorName { get; set; }
    }

    public class GetDetailCultureDTO
    {
        [Key]
        public int CultureId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Period { get; set; }
        public string? Continent { get; set; }
        public string? Country { get; set; }
        public string CreatorName { get; set; }

    }

    public class CreateCultureDTO
    {
        public string Title { get; set; }
        //public string Content { get; set; }
        public string ThumbnailImage { get; set; }
        public string Description { get; set; }
        public string Period { get; set; }
        public string? Continent { get; set; }
        public string? Country { get; set; }
        public string? FileName { get; set; }
        public int CreatorId { get; set; }

    }

    public class CreateCultureResponseDTO
    {
        public int CultureId { get; set; }
        public bool IsActive { get; set; }
        public string Message { get; set; }
    }

    public class CultureQueryParameters
    {
        const int maxPageSize = 50;
        public int Page { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value > maxPageSize ? maxPageSize : value; }
        }

        public string? Country { get; set; }
        public string? Continent { get; set; }
        public string? SearchTerm { get; set; }  
        public string? SortColumn { get; set; }  
        public string? SortOrder { get; set; } = "asc";  // "asc" or "desc" for sorting order
    }
}
