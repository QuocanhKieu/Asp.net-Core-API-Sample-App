using System.ComponentModel.DataAnnotations.Schema;

namespace T2305M_API.DTO.UserEvent
{

    public class UserEventResponseDTO
    {
        public int EventId { get; set; }
        public string Message { get; set; }
    }
    public class GetUserEventDTO
    {
        public int OrderId { get; set; }
        public int EventId { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
    }
    public class GetDetailUserEventDTO
    {
        public int EventId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; } // number of ticket
        public decimal? TicketPrice { get; set; }
        public List<string> EventTicketCodes { get; set; }
        public string Status { get; set; } = "PENDING"; // orderStatus
        public string Thumbnail { get; set; }
        public string Organizer { get; set; }
        public bool IsHostOnline { get; set; } = false;
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

        //EventId, Title, Total Tickets, ticket code, price, order status , lấy hết event
    }

    public class UserEventQueryParameters
    {
        const int maxPageSize = 50;
        public int Page { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value > maxPageSize ? maxPageSize : value; }
        }
        public string? SearchTerm { get; set; }
        public string? SortColumn { get; set; } = "OrderDate";
        public string? SortOrder { get; set; } = "asc";
    }
}
