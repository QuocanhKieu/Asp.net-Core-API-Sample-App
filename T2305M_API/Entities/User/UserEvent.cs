using T2305M_API.Entities;

public class UserEvent
{
    public int UserId { get; set; }
    public virtual User? User { get; set; }  // Navigation property to User

    public int EventId { get; set; }
    public virtual Event? Event { get; set; }  // Navigation property to Event

    public DateTime? CreatedAt { get; set; }
}