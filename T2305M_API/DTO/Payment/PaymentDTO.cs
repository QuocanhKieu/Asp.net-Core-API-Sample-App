using System.ComponentModel.DataAnnotations;

namespace T2305M_API.DTO
{
    public class UpdatePaymentStatusDTO 
    {
        public int PaymentId { get; set; }
        public string? Status { get; set; }
    }
    public class UpdatePaymentStatusResonse
    {
        public bool IsSuccess { get; set; }

        public string PaymentStatus { get; set; }

        public string Message { get; set; }
        public string UserEmail { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
