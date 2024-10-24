using System.ComponentModel.DataAnnotations;
using T2305M_API.Entities;

namespace T2305M_API.DTO.Book
{
    public class GetBasicBookDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string ThumbnailImage { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public bool IsRecommended { get; set; }
        public decimal Price { get; set; }
        public int CopiesSold { get; set; } = 0;
        public DateTime ReleaseDate { get; set; }
        public List<TagDTO> Tags { get; set; }
    }

    public class TagDTO
    {
        public int TagId { get; set; }
        public string Name { get; set; }
    }

    public class CreateBookDTO
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string ThumbnailImage { get; set; }
        public bool IsRecommended { get; set; }
        public decimal Price { get; set; }
        public int CopiesSold { get; set; }// for fake data
        public DateTime ReleaseDate { get; set; }
        public int CreatorId { get; set; }  // ID of the Creator
        public List<int>? TagIds { get; set; }  // Optional list of tag IDs to associate with the book
    }

    public class CreateBookResponseDTO
    {
        public int BookId { get; set; }
        public bool IsActive { get; set; }
        public string Message { get; set; }
    }

    public class UpdateBookDTO : CreateBookDTO
    {
        public int BookId { get; set; }
    }

    public class BookQueryParameters
    {
        const int maxPageSize = 50;

        public int Page { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value > maxPageSize ? maxPageSize : value; }
        }

        public bool IsRecommended { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortColumn { get; set; }
        public string? SortOrder { get; set; } = "asc";  // "asc" or "desc" for sorting order

        // Filter by multiple tags
        // This will hold the raw input tags
        public string? RawTags { get; set; } // E.g., "General Interest, Global History"

        // Parsed list of tags
        public List<string>? Tags { get; set; }

        public void ParseTags()
        {
            if (!string.IsNullOrWhiteSpace(RawTags))
            {
                Tags = RawTags.Split(',')
                              .Select(tag => tag.Trim()) // Trim whitespace
                              .ToList();
            }
        }
    }    // List of tags to filter books by
    
}
