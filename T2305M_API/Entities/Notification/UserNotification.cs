namespace T2305M_API.Entities.Notification
{
    public class UserNotification
    {
        public int UserNotificationId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false; // Default to unread
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default to the current time
        public int UserId { get; set; } // Reference to the user
        public virtual User? User { get; set; } // Reference to the user
    }

}
