using System.ComponentModel.DataAnnotations.Schema;

namespace T2305M_API.DTO
{
    public class GetBasicOrderDTO
    {
        public int OrderId { get; set; }
        public int EventId { get; set; }
        //public int PaymentId { get; set; } 
        public string UserName { get; set; }
        public string EventThumbnail { get; set; }
        public DateTime OrderDate { get; set; }
        public int TicketQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderStatus { get; set; }
        public string EventTitle { get; set; }
    }


    public class UpdateOrderStatusDTO
    {
        public int OrderId { get; set; }
        public string? Status { get; set; }
    }
    public class UpdateOrderStatusResponse
    {
        public bool IsSuccess { get; set; }

        public string OrderStatus { get; set; }
        public List<string> TicketCodes { get; set; }
        public string Message { get; set; }
        public string UserEmail { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }

    }


    public class OrderQueryParameters
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
        public string? SearchTerm { get; set; }
        public string? SortColumn { get; set; } = "OrderDate";
        public string? SortOrder { get; set; } = "asc";
    }
}
