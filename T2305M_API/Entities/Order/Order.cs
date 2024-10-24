using System.ComponentModel.DataAnnotations;

namespace T2305M_API.Entities
{
    public class Order
    {
        [Key]// auto increament
        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public virtual User? User { get; set; }
        public int? EventId { get; set; }
        public int Quantity { get; set; } // Number of tickets
        public virtual Event? Event { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "PENDING";
        public virtual ICollection<EventTicket>? EventTickets { get; set; }
        public int? PaymentId { get; set; }
        public virtual Payment? Payment { get; set; }
    }

}
