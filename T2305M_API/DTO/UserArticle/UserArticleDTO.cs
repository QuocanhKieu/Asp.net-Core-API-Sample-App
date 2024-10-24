using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using T2305M_API.Entities;

namespace T2305M_API.DTO.UserArticle
{
    public class GetBasicUserArticleDTO
    {
        public int UserArticleId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ThumbnailImage { get; set; }
        public bool IsPromoted { get; set; }
        public int UserId { get; set; }// có thể dùng để link tới trang profile của User này
        public string UserName { get; set; }  
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }  // Default value
        public List<UserArticleTagDTO>? UserArticleTags { get; set; }
    }

    public class UserArticleTagDTO
    {
        public int UserArticleTagId { get; set; }
        public string Name { get; set; }
    }

    public class GetDetailUserArticleDTO
    {
        public int UserArticleId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "LONGTEXT")]
        public string Content { get; set; }
        public string ThumbnailImage { get; set; }
        public bool IsPromoted { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Status { get; set; } // Default value

    }

    public class CreateUserArticleDTO
    {
        public IFormFile file { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "LONGTEXT")]
        public string Content { get; set; }
        public string? ThumbnailImage { get; set; }
        public List<int>? UserArticleTagIds { get; set; } = new List<int>(); // Optional list of tag IDs to associate with the book
    }
    public class UpdateUserArticleDTO : CreateUserArticleDTO
    {
        public int UserArticleId { get; set; }
    }

    public class CreateUserArticleResponseDTO
    {
        public int UserArticleId { get; set; }
        public string Status{ get; set; }
        public string Message { get; set; }
    }
    public class UpdateUserArticleResponseDTO : CreateUserArticleResponseDTO
    {
    }

    public class ChangeUserArticleStatusResponseDTO : CreateUserArticleResponseDTO
    {
      
    }

    public class UserArticleQueryParameters
    {
        const int maxPageSize = 50;
        private int _pageSize = 10;
        public int Page { get; set; } = 1;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value < 1 ? 1 : (value > maxPageSize ? maxPageSize : value); }
        }
        public int? UserId { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortColumn { get; set; } = "CreatedAt";
        public string? SortOrder { get; set; } = "desc";  // "asc" or "desc" for sorting order
        public string? RawTagIds { get; set; }  // Accepts raw string of tag ids (e.g., "1,2,3")
        public List<int>? tagIdList { get; set; } = new List<int>();
        public List<string>? invalidIds { get; set; } = new List<string>();

        public void ParseTagIds()
        {
            if (!string.IsNullOrWhiteSpace(RawTagIds))
            {
                var ids = RawTagIds.Split(',');

                foreach (var id in ids)
                {
                    if (int.TryParse(id.Trim(), out var tagId))
                    {
                        tagIdList.Add(tagId);  
                    }
                    else
                    {
                        invalidIds.Add(id.Trim());  
                    }
                }
            }
        }

        public UserArticleQueryParameters()
        {
            ParseTagIds();
        }
    }

}
