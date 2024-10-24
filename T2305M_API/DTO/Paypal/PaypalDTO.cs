using System.ComponentModel.DataAnnotations;

namespace T2305M_API.DTO
{
    public class OrderDTO
    {
        public int EventId { get; set; }
        public decimal TicketPrice { get; set; } // Price per ticket
        public int Quantity { get; set; } // Number of tickets
        public decimal TotalAmount => TicketPrice * Quantity; // Total amount
        public string? PaypalOrderId { get; set; }
        public string? PaypalPayerId { get; set; }
        public string? PaypalPaymentId { get; set; }
        public string? PaymentMethod { get; set; }
    }

    public class PaypalHandleResponse
    {
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
