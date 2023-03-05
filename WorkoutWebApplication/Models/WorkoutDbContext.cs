using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WorkoutWebApplication.Models;

public partial class WorkoutDbContext : DbContext
{
    public WorkoutDbContext()
    {
    }

    public WorkoutDbContext(DbContextOptions<WorkoutDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FocusArea> FocusAreas { get; set; }

    public virtual DbSet<Plan> Plans { get; set; }

    public virtual DbSet<PlansWorkout> PlansWorkouts { get; set; }

    public virtual DbSet<Sportsman> Sportsmen { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<WeekDay> WeekDays { get; set; }

    public virtual DbSet<Workout> Workouts { get; set; }

    public virtual DbSet<WorkoutType> WorkoutTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server= DESKTOP-TB0G3NE\\SQLEXPRESS; Database=WorkoutDb; Trusted_Connection=True; TrustServerCertificate=True; ");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlansWorkout>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Plans_Workouts");

            entity.Property(e => e.PlanId).HasColumnName("PlanID");

            entity.HasOne(d => d.Plan).WithMany(p => p.PlansWorkouts)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlansWorkouts_Plans");

            entity.HasOne(d => d.WeekDay).WithMany(p => p.PlansWorkouts)
                .HasForeignKey(d => d.WeekDayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlansWorkouts_WeekDays");

            entity.HasOne(d => d.Workout).WithMany(p => p.PlansWorkouts)
                .HasForeignKey(d => d.WorkoutId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlansWorkouts_Workouts");
        });

        modelBuilder.Entity<Sportsman>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Sportsmen");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.Property(e => e.Date).HasColumnType("date");
            

            entity.HasOne(d => d.Plan).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subscriptions_Plans");

            entity.HasOne(d => d.Sportsman).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.SportsmanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subscriptions_Sportsmen");
        });

        modelBuilder.Entity<Workout>(entity =>
        {
            entity.Property(e => e.Faid).HasColumnName("FAId");
            entity.Property(e => e.Wtid).HasColumnName("WTId");

            entity.HasOne(d => d.Fa).WithMany(p => p.Workouts)
                .HasForeignKey(d => d.Faid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Workouts_FocusAreas");

            entity.HasOne(d => d.Wt).WithMany(p => p.Workouts)
                .HasForeignKey(d => d.Wtid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Workouts_WorkoutTypes");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
