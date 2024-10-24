using System.ComponentModel.DataAnnotations;

namespace T2305M_API.Entities
{
    public class EventTicket
    {
        [Key]// auto increament
        public int EventTicketId { get; set; }

        // The ticket code is set when a new ticket is created
        public string EventTicketCode { get; set; }

        public decimal TicketPrice { get; set; }

        public int? UserId { get; set; }
        public virtual User? User { get; set; }

        public int? EventId { get; set; }
        public virtual Event? Event { get; set; }

        public int? PaymentId { get; set; }
        public virtual Payment? Payment { get; set; }

        public int? OrderId { get; set; }
        public virtual Order? Order { get; set; }

        // Constructor to generate a readable, concise ticket code
        public EventTicket()
        {
            // Generate a unique ticket code in the format EVT-XXXX-XXXX where X is alphanumeric
            EventTicketCode = GenerateUniqueTicketCode();
        }

        private string GenerateUniqueTicketCode()
        {
            // Use a short portion of a GUID combined with a random alphanumeric string
            string guidSegment = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            string randomSegment = GenerateRandomAlphanumericString(6).ToUpper();

            // Format as EVT-XXXX-XXXX for better readability
            return $"EVT-{guidSegment}-{randomSegment}";
        }

        private string GenerateRandomAlphanumericString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }

            return new string(result);
        }
    }
}
