using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using T2305M_API.Entities;

namespace T2305M_API.DTO.Event
{
    public class GetBasicEventDTO
    {
        // Event Details
        [Key]
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Organizer { get; set; }
        public string Thumbnail { get; set; }
        public bool IsHostOnline { get; set; }
        public decimal? TicketPrice { get; set; }
        public DateTime? SaleDueDate { get; set; }  // before StartDate at least one day
        public int MaxTickets { get; set; }
        public int CurrentTickets { get; set; } = 0;
        public bool IsPromoted { get; set; } = false;
        public string? WebsiteUrl { get; set; }
        // Event Time
        public DateTime StartDate { get; set; }  // Start date of the event // sort by default
        public DateTime? EndDate { get; set; }   // End date of the event, can be null if it's a single-day event
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        // Location Fileds
        public string Continent { get; set; }
        public string Country { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }


    public class GetDetailEventDTO : GetBasicEventDTO
    {
        public string Content { get; set; }
    }

    public class CreateEventDTO
    {
        public IFormFile formFile { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Organizer { get; set; } = "";
        public string Content { get; set; } = "";
        public bool IsHostOnline { get; set; } = false;
        public decimal? TicketPrice { get; set; } = 0;
        public DateTime? SaleDueDate { get; set; }  // before StartDate at least one day
        public int MaxTickets { get; set; }
        public bool IsPromoted { get; set; }
        public string? WebsiteUrl { get; set; }
        // Event Time
        public DateTime StartDate { get; set; }  // Start date of the event // sort by default
        public DateTime? EndDate { get; set; }   // End date of the event, can be null if it's a single-day event
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        // Location Fileds
        public string Continent { get; set; }
        public string Country { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
        public int? UserId { get; set; }
    }

    public class CreateEventResponseDTO
    {
        public int EventId { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }

    public class UpdateEventDTO : CreateEventDTO
    {
        public int EventId { get; set; }
        public string Status { get; set; } = "PENDING";
    }

    public class UpdateEventResponseDTO : CreateEventResponseDTO
    {
    }

    public class EventQueryParameters
    {
        const int maxPageSize = 50;
        public int Page { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value > maxPageSize ? maxPageSize : value; }
        }
        public string? Status { get; set; } 
        public string? Country { get; set; }
        public string? Continent { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortColumn { get; set; } 
        public string? SortOrder { get; set; } = "asc";  // "asc" or "desc" for sorting order
    }
}
