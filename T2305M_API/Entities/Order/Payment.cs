using System.ComponentModel.DataAnnotations;

namespace T2305M_API.Entities

{
    public class Payment
    {
        [Key] // auto increment
        public int PaymentId { get; set; }
        public int? OrderId { get; set; }
        public virtual Order? Order { get; set; }
        public int? UserId { get; set; }
        public virtual User? User{ get; set; }
        public string? PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "COMPLETE";
        public DateTime PaymentCreateDate { get; set; } = DateTime.Now;
        public DateTime? CompletePaymentDate { get; set; }
        public string? PaypalOrderId { get; set; }
        public string? PaypalPayerId { get; set; }
        public string? PaypalPaymentId { get; set; }
    }   
}
