using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StrawberryHub.Models;

namespace StrawberryHub.Services;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Article> Article { get; set; } = default!;

    public virtual DbSet<EmergencySupport> EmergencySupport { get; set; } = default!;

    public virtual DbSet<Goal> Goal { get; set; } = default!;

    public virtual DbSet<GoalType> GoalType { get; set; } = default!;

    public virtual DbSet<Rank> Rank { get; set; } = default!;

    public virtual DbSet<Reflection> Reflection { get; set; } = default!;

    public virtual DbSet<StrawberryHub.Models.Task> Task { get; set; } = default!;

    public virtual DbSet<User> User { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PK__Article__9C6270E88056FA81");

            entity.Property(e => e.ArticleContent).IsUnicode(false);
            entity.Property(e => e.PublishedDate).HasColumnType("datetime");

            entity.HasOne(d => d.GoalType).WithMany(p => p.Article)
                .HasForeignKey(d => d.GoalTypeId)
                .HasConstraintName("FK__Article__GoalTyp__3D5E1FD2");
        });

        modelBuilder.Entity<EmergencySupport>(entity =>
        {
            entity.HasKey(e => e.EmergencySupportId).HasName("PK__Emergenc__812C07C524050E3F");

            entity.Property(e => e.Message).IsUnicode(false);
            entity.Property(e => e.Timestamp).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.EmergencySupport)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Emergency__UserI__36B12243");
        });

        modelBuilder.Entity<Goal>(entity =>
        {
            entity.HasKey(e => e.GoalId).HasName("PK__Goal__8A4FFFD1D250A82C");

            entity.HasOne(d => d.GoalType).WithMany(p => p.Goal)
                .HasForeignKey(d => d.GoalTypeId)
                .HasConstraintName("FK__Goal__GoalTypeId__30F848ED");

            entity.HasOne(d => d.User).WithMany(p => p.Goal)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Goal__UserId__300424B4");
        });

        modelBuilder.Entity<GoalType>(entity =>
        {
            entity.HasKey(e => e.GoalTypeId).HasName("PK__GoalType__20722C9298E438C7");

            entity.HasIndex(e => e.Type, "UQ__GoalType__F9B8A48BADB79F6E").IsUnique();

            entity.Property(e => e.Type).HasMaxLength(50);
        });

        modelBuilder.Entity<Rank>(entity =>
        {
            entity.HasKey(e => e.RankId).HasName("PK__Rank__B37AF876153D76AB");

            entity.HasIndex(e => e.RankName, "UQ__Rank__1FC2E658BE35335E").IsUnique();

            entity.Property(e => e.RankName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Reflection>(entity =>
        {
            entity.HasKey(e => e.ReflectionId).HasName("PK__Reflecti__5D73947A233E507B");

            entity.Property(e => e.Content).IsUnicode(false);
            entity.Property(e => e.Date).HasColumnType("date");

            entity.HasOne(d => d.User).WithMany(p => p.Reflection)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Reflectio__UserI__33D4B598");
        });

        modelBuilder.Entity<StrawberryHub.Models.Task>(entity => // Use fully qualified name here
        {
            entity.HasKey(e => e.TaskId).HasName("PK__Task__7C6949B1F4F061F9");

            entity.Property(e => e.IsCompleted).HasDefaultValueSql("((0))");
            entity.Property(e => e.TaskDescription).IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Task)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Task__UserId__398D8EEE");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C83DD2A55");

            entity.HasIndex(e => e.Username, "UQ__User__536C85E427FA4252").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Points).HasDefaultValueSql("((0))");
            entity.Property(e => e.RankId).HasDefaultValueSql("((1))");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Rank).WithMany(p => p.User)
                .HasForeignKey(d => d.RankId)
                .HasConstraintName("FK__User__RankId__2A4B4B5E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
