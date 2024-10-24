using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using T2305M_API.Entities.Notification;

namespace T2305M_API.Entities;

public partial class T2305mApiContext : DbContext
{
    public static string ConnectionString;
    public T2305mApiContext()
    {
    }

    public T2305mApiContext(DbContextOptions<T2305mApiContext> options)
        : base(options)
    {
    }

    public DbSet<Creator> Creator { get; set; }
    public DbSet<Event> Event { get; set; }
    public DbSet<Culture> Culture { get; set; }
    public DbSet<History> History { get; set; }
    public DbSet<Book> Book { get; set; }
    public DbSet<Tag> Tag { get; set; }
    public DbSet<BookTag> BookTag { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<UserEvent> UserEvent { get; set; }
    public DbSet<UserArticle> UserArticle { get; set; }
    public DbSet<UserArticleTag> UserArticleTag { get; set; }
    public DbSet<UserArticleUserArticleTag> UserArticleUserArticleTag { get; set; }
    public DbSet<UserNotification> UserNotification { get; set; }
    public DbSet<Image> Image { get; set; }
    public DbSet<EventTicket> EventTicket { get; set; }
    public DbSet<Order> Order { get; set; }
    public DbSet<Payment> Payment{ get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer(ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEvent>()
        .HasKey(ue => new { ue.UserId, ue.EventId });
        modelBuilder.Entity<UserFavoriteEvent>()
.HasKey(ue => new { ue.UserId, ue.EventId });
        modelBuilder.Entity<BookTag>()
        .HasKey(bt => new { bt.BookId, bt.TagId });

        modelBuilder.Entity<UserArticleUserArticleTag>()
        .HasKey(bt => new { bt.UserArticleId, bt.UserArticleTagId });

        modelBuilder.Entity<Event>()
        .Property(e => e.TicketPrice)
        .HasPrecision(18, 2);

        modelBuilder.Entity<Book>()
        .Property(e => e.Price)
        .HasPrecision(18, 2);


        modelBuilder.Entity<Order>()
          .HasOne(o => o.Payment)
          .WithOne(p => p.Order)
          .HasForeignKey<Payment>(p => p.OrderId);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}