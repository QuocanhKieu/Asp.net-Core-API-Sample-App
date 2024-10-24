using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using T2305M_API.DTO.Book;
using T2305M_API.DTO.Event;
using T2305M_API.DTO.UserArticle;
using T2305M_API.DTO.UserEvent;
using T2305M_API.DTO.Event;
using T2305M_API.Entities;


namespace T2305M_API.DTO.User
{
    public class GetBasicUserDTO
    {
        [Key]
        public int UserId { get; set; }
        public bool IsActive { get; set; } = true;  // Account activation status
    }

    public class GetDetailUserDTO
    {
        [Key]
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Education { get; set; }  // This can be a simple string or a list of degrees
        public string? ShortBiography { get; set; }
        [Column(TypeName = "LONGTEXT")]
        public string? LongBiography { get; set; }
        public string? PhotoUrl { get; set; }
        public int? Age { get; set; }
        // Social Media Accounts
        public string? Facebook { get; set; }
        public string? LinkedIn { get; set; }
        public string? Twitter { get; set; }

        // Personal Website
        public string? PersonalWebsiteUrl { get; set; }
        public string? PersonalWebsiteTitle { get; set; }
        public bool ReceiveNotifications { get; set; } = true;
        public bool IsActive { get; set; } = true;  // Account activation status

        public List<GetBasicEventDTO>? BasicUserSavedEvents { get; set; }
        public List<GetBasicUserArticleDTO>? BasicUserArticles{ get; set; }

    }
    //user click vào bài viết nếu là người tạo ra bài viết thì sẽ hiện ra nút edit, xóa
    // vào user profile nếu là user id trùng thì hiển thị form  cho phép edit ko trùng thì chỉ cho xem thông tin 

    public class UpdateAvatarResponseDTO
    {
        public int UserId { get; set; }
        public string FilePath { get; set; }
        public string Message { get; set; }
    }

    public class UpdateUserDTO 
    {
        //public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Education { get; set; }  // This can be a simple string or a list of degrees
        public string? ShortBiography { get; set; }
        [Column(TypeName = "NVARCHAR(MAX)")] // or "VARCHAR(MAX)" if you prefer
        public string? LongBiography { get; set; }
        public int? Age { get; set; }


        // Social Media Accounts
        public string? Facebook { get; set; }
        public string? LinkedIn { get; set; }
        public string? Twitter { get; set; }

        // Personal Website
        public string? PersonalWebsiteUrl { get; set; }
        public string? PersonalWebsiteTitle { get; set; }
        public bool ReceiveNotifications { get; set; } = true;
    }
    //for the photoUrl has specific method for them

    public class UpdateUserResponseDTO
    {
        public int UserId { get; set; }
        public bool IsActive { get; set; }
        public string Message { get; set; }
    }
    public class UserQueryParameters
    {
        const int maxPageSize = 50;
        public int Page { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value > maxPageSize ? maxPageSize : value; }
        }
        public bool isToday { get; set; }
        public bool isThisWeek { get; set; }
        public bool isNextWeek { get; set; }
        public bool IsHostOnline { get; set; }
        public string? Country { get; set; }
        public string? Continent { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortColumn { get; set; }
        public string? SortOrder { get; set; } = "asc";  // "asc" or "desc" for sorting order
    }
}
