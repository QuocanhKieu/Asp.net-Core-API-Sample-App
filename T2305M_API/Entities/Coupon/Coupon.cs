using System.ComponentModel.DataAnnotations;

namespace T2305M_API.Entities
{
    public class Coupon
    {
        [Key]
        public int CouponId { get; set; }
        public string Code { get; set; } // e.g., "DISCOUNT10"
        public decimal DiscountAmount { get; set; } // Fixed amount or percentage
        public string Status { get; set; } = "PENDING"; // To check if the coupon is active
        public DateTime ExpiryDate { get; set; } // Expiry date of the coupon
        public int MaxUsage { get; set; } // Maximum times the coupon can be used
        public int CurrentUsage { get; set; } = 0;// Current number of times the coupon has been used
    }
}
