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

    public virtual DbSet<StrawberryArticle> StrawberryArticle { get; set; }

    public virtual DbSet<StrawberryEmergencySupport> StrawberryEmergencySupport { get; set; }

    public virtual DbSet<StrawberryGoal> StrawberryGoal { get; set; }

    public virtual DbSet<StrawberryGoalType> StrawberryGoalType { get; set; }

    public virtual DbSet<StrawberryLikeComment> StrawberryLikeComment { get; set; }

    public virtual DbSet<StrawberryRank> StrawberryRank { get; set; }

    public virtual DbSet<StrawberryReflection> StrawberryReflection { get; set; }

    public virtual DbSet<StrawberryTask> StrawberryTask { get; set; }

    public virtual DbSet<StrawberryUser> StrawberryUser { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StrawberryArticle>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PK__Strawber__9C6270E840FD6ED4");

            entity.Property(e => e.ArticleContent).IsUnicode(false);
            entity.Property(e => e.Picture)
               .HasMaxLength(70)
               .IsUnicode(false);
            entity.Property(e => e.PublishedDate).HasColumnType("datetime");
            modelBuilder.Entity<StrawberryArticle>()
                .Ignore(e => e.Photo);

            entity.HasOne(d => d.GoalType).WithMany(p => p.StrawberryArticles)
                .HasForeignKey(d => d.GoalTypeId)
                .HasConstraintName("FK__Strawberr__GoalT__72C60C4A");

            entity.HasOne(d => d.StrawberryUser).WithMany(p => p.StrawberryArticle)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Strawberr__UserI__2739D489");
        });

        modelBuilder.Entity<StrawberryEmergencySupport>(entity =>
        {
            entity.HasKey(e => e.EmergencySupportId).HasName("PK__Strawber__812C07C5E1A56D8D");

            entity.Property(e => e.Message).IsUnicode(false);
            entity.Property(e => e.Timestamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<StrawberryGoal>(entity =>
        {
            entity.HasKey(e => e.GoalId).HasName("PK__Strawber__8A4FFFD171B438F2");
        });

        modelBuilder.Entity<StrawberryGoalType>(entity =>
        {
            entity.HasKey(e => e.GoalTypeId).HasName("PK__Strawber__20722C92A4940FF9");

            entity.HasIndex(e => e.Type, "UQ__Strawber__F9B8A48B13E8CECF").IsUnique();

            entity.Property(e => e.Type).HasMaxLength(50);
        });

        modelBuilder.Entity<StrawberryLikeComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Strawber__C3B4DFCA69D54E85");

            entity.Property(e => e.CommentText).IsUnicode(false);
            entity.Property(e => e.CommentTimestamp).HasColumnType("datetime");
            entity.Property(e => e.LikeTimestamp).HasColumnType("datetime");
            entity.Property(e => e.Likes).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.StrawberryArticle).WithMany(p => p.StrawberryLikeComments)
                .HasForeignKey(d => d.ArticleId)
                .HasConstraintName("FK__Strawberr__Artic__2CF2ADDF");

            entity.HasOne(d => d.StrawberryUser).WithMany(p => p.StrawberryLikeComment)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Strawberr__UserI__2BFE89A6");
        });

        modelBuilder.Entity<StrawberryRank>(entity =>
        {
            entity.HasKey(e => e.RankId).HasName("PK__Strawber__B37AF87686DBA7B8");

            entity.HasIndex(e => e.RankName, "UQ__Strawber__1FC2E658EF30FBB1").IsUnique();

            entity.Property(e => e.RankName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<StrawberryReflection>(entity =>
        {
            entity.HasKey(e => e.ReflectionId).HasName("PK__Strawber__5D73947ADF9ADBDA");

            entity.Property(e => e.Content).IsUnicode(false);
            entity.Property(e => e.Date).HasColumnType("date");
        });

        modelBuilder.Entity<StrawberryTask>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__Strawber__7C6949B1ADE6F83B");

            entity.Property(e => e.IsCompleted).HasDefaultValueSql("((0))");
            entity.Property(e => e.TaskDescription).IsUnicode(false);
        });

        modelBuilder.Entity<StrawberryUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__tmp_ms_x__1788CC4CEE66DDA2");

            entity.HasIndex(e => e.Username, "UQ__tmp_ms_x__536C85E4DBCE093D").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Otp)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.OtpCount).HasDefaultValueSql("((0))");
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Points).HasDefaultValueSql("((0))");
            entity.Property(e => e.RankId).HasDefaultValueSql("((1))");
            entity.Property(e => e.TelegramId)
               .HasMaxLength(50)
               .IsUnicode(false);
            entity.Property(e => e.UserRole)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
            //entity.Property(e => e.GoalTypeId)
                //.HasMaxLength(50)
                //.IsUnicode(false);

            //entity.HasOne(d => d.StrawberryGoalType).WithMany(p => p.StrawberryUsers)
                //.HasForeignKey(d => d.GoalTypeId)
                //.HasConstraintName("FK__Strawberr__GoalT__7F2BE32F");

            entity.HasOne(d => d.StrawberryRank).WithMany(p => p.StrawberryUsers)
                .HasForeignKey(d => d.RankId)
                .HasConstraintName("FK__Strawberr__RankI__04E4BC85");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
