using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ESMS_Data.Models
{
    public partial class ESMSContext : DbContext
    {
        public ESMSContext()
        {
        }

        public ESMSContext(DbContextOptions<ESMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<ExamSchedule> ExamSchedules { get; set; }
        public virtual DbSet<ExamTime> ExamTimes { get; set; }
        public virtual DbSet<Participation> Participations { get; set; }
        public virtual DbSet<Registration> Registrations { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Slot> Slots { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Department");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ExamSchedule>(entity =>
            {
                entity.HasKey(e => new { e.Idt, e.SubjectId, e.RoomNumber });

                entity.ToTable("ExamSchedule");

                entity.Property(e => e.Idt).HasColumnName("IDT");

                entity.Property(e => e.SubjectId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SubjectID");

                entity.Property(e => e.RoomNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Form)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Proctor)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdtNavigation)
                    .WithMany(p => p.ExamSchedules)
                    .HasForeignKey(d => d.Idt)
                    .HasConstraintName("FK_ExamSchedule_ExamTime");

                entity.HasOne(d => d.RoomNumberNavigation)
                    .WithMany(p => p.ExamSchedules)
                    .HasForeignKey(d => d.RoomNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExamSchedule_Room");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.ExamSchedules)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExamSchedule_Subject");

                entity.HasOne(d => d.Registration)
                    .WithMany(p => p.ExamSchedules)
                    .HasForeignKey(d => new { d.Proctor, d.Idt })
                    .HasConstraintName("FK_ExamSchedule_Registration");
            });

            modelBuilder.Entity<ExamTime>(entity =>
            {
                entity.HasKey(e => e.Idt);

                entity.ToTable("ExamTime");

                entity.HasIndex(e => new { e.Date, e.Start, e.End }, "UK_ExamTime")
                    .IsUnique();

                entity.Property(e => e.Idt).HasColumnName("IDT");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.PublishDate).HasColumnType("date");

                entity.Property(e => e.IsPublic)
                    .HasColumnName("IsPublic")
                    .HasColumnType("bit") 
                    .HasDefaultValue(false);

                entity.Property(e => e.Semester)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.SlotId).HasColumnName("SlotID");

                entity.HasOne(d => d.Slot)
                    .WithMany(p => p.ExamTimes)
                    .HasForeignKey(d => d.SlotId)
                    .HasConstraintName("FK_ExamTime_Slot");
            });

            modelBuilder.Entity<Participation>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.SubjectId, e.RoomNumber, e.Idt })
                    .HasName("PK_Student_Join_ExamSchedule");

                entity.ToTable("Participation");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SubjectId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SubjectID");

                entity.Property(e => e.RoomNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Idt).HasColumnName("IDT");

                entity.HasOne(d => d.UserNameNavigation)
                    .WithMany(p => p.Participations)
                    .HasForeignKey(d => d.UserName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Student_Join_ExamSchedule_User");

                entity.HasOne(d => d.ExamSchedule)
                    .WithMany(p => p.Participations)
                    .HasForeignKey(d => new { d.Idt, d.SubjectId, d.RoomNumber })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Participation_ExamSchedule");
            });

            modelBuilder.Entity<Registration>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.Idt });

                entity.ToTable("Registration");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Idt).HasColumnName("IDT");

                entity.HasOne(d => d.IdtNavigation)
                    .WithMany(p => p.Registrations)
                    .HasForeignKey(d => d.Idt)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Lecturer_Register_Exam_ExamTime");

                entity.HasOne(d => d.UserNameNavigation)
                    .WithMany(p => p.Registrations)
                    .HasForeignKey(d => d.UserName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Lecturer_Register_Exam_User");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.Number);

                entity.ToTable("Room");

                entity.Property(e => e.Number)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Slot>(entity =>
            {
                entity.ToTable("Slot");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("Subject");

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ID");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserName);

                entity.ToTable("User");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Idcard)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("IDCard");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.RollNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Department");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Role");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
