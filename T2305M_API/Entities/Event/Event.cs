using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using T2305M_API.Entities;


namespace T2305M_API.Entities
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public string Description { get; set; }
        public string Organizer { get; set; }
        [Column(TypeName = "NVARCHAR(MAX)")] 
        public string Content { get; set; }
        public bool IsHostOnline { get; set; } = false;
        public decimal? TicketPrice { get; set; }
        public DateTime? SaleDueDate { get; set; }  
        public int MaxTickets { get; set; } 
        public int CurrentTickets { get; set; } = 0;
        public bool IsPromoted { get; set; } = false;
        public string? WebsiteUrl { get; set; }
        public DateTime StartDate { get; set; }  
        public DateTime? EndDate { get; set; }   
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Continent { get; set; }
        public string Country { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
        public int UserId { get; set; }
        public virtual User? User { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string Status { get; set; } = "PENDING"; // APPROVED, CANCELED
        public virtual ICollection<UserEvent>? UserEvents { get; set; }  
        public virtual ICollection<EventTicket>? EventTickets { get; set; }  
        public virtual ICollection<Order>? Orders { get; set; }  

    }
}
