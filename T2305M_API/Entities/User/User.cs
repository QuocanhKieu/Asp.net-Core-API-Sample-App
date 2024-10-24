using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using T2305M_API.Entities.Notification;

namespace T2305M_API.Entities;

public partial class User
{
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Password { get; set; }
    public string Role { get; set; } = "USER"; // USER/ ADMIN /
    public int? Age { get; set; }
    public string? Education { get; set; }  // This can be a simple string or a list of degrees
    public string? ShortBiography { get; set; }
    [Column(TypeName = "NVARCHAR(MAX)")] // or "VARCHAR(MAX)" if you prefer
    public string? LongBiography { get; set; }
    public string? PhotoUrl { get; set; } = "/uploads/avatars/default-avatar.png";
    public string? Facebook { get; set; }
    public string? LinkedIn { get; set; }
    public string? Twitter { get; set; }
    public string? PersonalWebsiteUrl { get; set; }
    public string? PersonalWebsiteTitle { get; set; }
    public bool ReceiveNotifications { get; set; } = true;
    public bool IsActive { get; set; } = true;  // Account activation status
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public virtual ICollection<UserEvent>? UserEvents { get; set; }  // Navigation property for many-to-many
    public virtual ICollection<UserArticle>? UserArticles { get; set; }  // Navigation property for many-to-many
    public virtual ICollection<UserNotification>? UserNotifications { get; set; }  // Navigation property for many-to-many
    public virtual ICollection<EventTicket>? EventTickets { get; set; }  // Navigation property for many-to-many 
    public virtual ICollection<Order>? Orders { get; set; }  // Navigation property for many-to-many 

}
