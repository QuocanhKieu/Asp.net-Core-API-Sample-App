using System.ComponentModel.DataAnnotations;

namespace T2305M_API.DTO.History
{
    public class GetBasicHistoryDTO
    {
        public int HistoryId { get; set; }
        public string Title { get; set; }
        public string ThumbnailImage { get; set; }
        public string Description { get; set; }
        public string Type { get; } = "History";
        public string CreatorName { get; set; }
    }

    public class GetDetailHistoryDTO
    {
        [Key]
        public int HistoryId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Period { get; set; }
        public string? Continent { get; set; }
        public string? Country { get; set; }
        public string CreatorName { get; set; }
    }

    public class CreateHistoryDTO
    {
        //will manually validate fields
        public string Title { get; set; }
        //public string Content { get; set; }
        public string ThumbnailImage { get; set; }
        public string Description { get; set; }
        public string Period { get; set; }
        public string? Continent { get; set; }
        public string? Country { get; set; }
        public int TimeOrder { get; set; }
        public string? FileName { get; set; }
        public int CreatorId { get; set; }

    }

    public class CreateHistoryResponseDTO
    {
        //will manually validate fields
        public int HistoryId { get; set; }
        public bool IsActive { get; set; }
        public string Message { get; set; }
    }

    public class HistoryQueryParameters
    {
        const int maxPageSize = 50;
        public int Page { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value > maxPageSize ? maxPageSize : value; }
        }
        public string? Period { get; set; } 
        public string? Country { get; set; } 
        public string? Continent { get; set; } 
        public string? SearchTerm { get; set; } 

        // New properties for sorting
        public string? SortColumn { get; set; }  
        public string? SortOrder { get; set; } = "asc";  

    }
}
