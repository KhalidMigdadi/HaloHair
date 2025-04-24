using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HaloHair.Models;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AppointmentService> AppointmentServices { get; set; }

    public virtual DbSet<Barber> Barbers { get; set; }

    public virtual DbSet<BarberService> BarberServices { get; set; }

    public virtual DbSet<BarberWorkImage> BarberWorkImages { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingService> BookingServices { get; set; }

    public virtual DbSet<BookingTemp> BookingTemps { get; set; }

    public virtual DbSet<BookingsHistory> BookingsHistories { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<JobApplication> JobApplications { get; set; }

    public virtual DbSet<PaymentInfo> PaymentInfos { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Salon> Salons { get; set; }

    public virtual DbSet<SalonBarber> SalonBarbers { get; set; }

    public virtual DbSet<SalonImage> SalonImages { get; set; }

    public virtual DbSet<SelectedService> SelectedServices { get; set; }

    public virtual DbSet<SelectedServiceTemp> SelectedServiceTemps { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<TimeSlot> TimeSlots { get; set; }

    public virtual DbSet<TrainingCourse> TrainingCourses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vacancy> Vacancies { get; set; }

    public virtual DbSet<WorkingHour> WorkingHours { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-5U44ISQ;Database=SalonBookDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Appointm__3214EC07C63AC003");

            entity.Property(e => e.AppointmentDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Barber).WithMany(p => p.Appointments).HasForeignKey(d => d.BarberId);

            entity.HasOne(d => d.Salon).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.SalonId)
                .HasConstraintName("FK__Appointme__Salon__7C4F7684");

            entity.HasOne(d => d.Service).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__Appointme__Servi__7D439ABD");

            entity.HasOne(d => d.User).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Appointme__UserI__7B5B524B");
        });

        modelBuilder.Entity<AppointmentService>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Appointm__3214EC07242BE0FF");

            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ServiceName).HasMaxLength(255);

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentServices)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Appoi__02FC7413");

            entity.HasOne(d => d.Service).WithMany(p => p.AppointmentServices)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__Appointme__Servi__03F0984C");
        });

        modelBuilder.Entity<Barber>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Barbers__3214EC07779C86AF");

            entity.HasIndex(e => e.Email, "UQ__Barbers__A9D1053425C3F8EC").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.HireDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserType)
                .HasMaxLength(50)
                .HasDefaultValue("Barber");

            entity.HasOne(d => d.Salon).WithMany(p => p.Barbers)
                .HasForeignKey(d => d.SalonId)
                .HasConstraintName("FK__Barbers__SalonId__628FA481");
        });

        modelBuilder.Entity<BarberService>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BarberSe__3214EC07448A0518");

            entity.Property(e => e.BarberFirstName).HasMaxLength(100);
            entity.Property(e => e.BarberLastName).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SalonName).HasMaxLength(100);

            entity.HasOne(d => d.Barber).WithMany(p => p.BarberServices)
                .HasForeignKey(d => d.BarberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BarberSer__Barbe__71D1E811");

            entity.HasOne(d => d.Service).WithMany(p => p.BarberServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BarberSer__Servi__72C60C4A");
        });

        modelBuilder.Entity<BarberWorkImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BarberWo__3214EC07331D8C35");

            entity.Property(e => e.ImageUrl).HasMaxLength(255);

            entity.HasOne(d => d.Barber).WithMany(p => p.BarberWorkImages)
                .HasForeignKey(d => d.BarberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BarberWorkImages_BarberId");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bookings__3214EC077A2B09B4");

            entity.HasOne(d => d.Barber).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.BarberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_Barbers");

            entity.HasOne(d => d.TimeSlot).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.TimeSlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_TimeSlots");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_Users");
        });

        modelBuilder.Entity<BookingService>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookingS__3214EC07FDC56952");

            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ServiceName).HasMaxLength(100);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingServices)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookingServices_Bookings");

            entity.HasOne(d => d.Service).WithMany(p => p.BookingServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookingServices_Services");
        });

        modelBuilder.Entity<BookingTemp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookingT__3214EC072705F38B");

            entity.ToTable("BookingTemp");

            entity.Property(e => e.AppointmentTime).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ServiceName).HasMaxLength(255);
            entity.Property(e => e.UserId).HasMaxLength(450);
        });

        modelBuilder.Entity<BookingsHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bookings__3214EC07EC2C4911");

            entity.ToTable("BookingsHistory");

            entity.Property(e => e.BookingDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Barber).WithMany(p => p.BookingsHistories)
                .HasForeignKey(d => d.BarberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookingsHistory_Barbers");

            entity.HasOne(d => d.User).WithMany(p => p.BookingsHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookingsHistory_Users");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Favorite__3214EC078DDE2E81");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Salon).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.SalonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Favorites_Salons");

            entity.HasOne(d => d.User).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Favorites_Users");
        });

        modelBuilder.Entity<JobApplication>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__JobAppli__3214EC078A89CEC4");

            entity.ToTable("JobApplication");

            entity.Property(e => e.AppliedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.ResumeFilePath).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Vacancy).WithMany(p => p.JobApplications)
                .HasForeignKey(d => d.VacancyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_JobApplication_Vacancy");
        });

        modelBuilder.Entity<PaymentInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentI__3214EC073C3855E6");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentDetails).HasMaxLength(255);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);

            entity.HasOne(d => d.Appointment).WithMany(p => p.PaymentInfos)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PaymentInfos_Appointments");

            entity.HasOne(d => d.Barber).WithMany(p => p.PaymentInfos)
                .HasForeignKey(d => d.BarberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PaymentInfo_Barber");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reviews__3214EC07D3E7515B");

            entity.Property(e => e.Comment).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.BookingHistory).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookingHistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__Booking__245D67DE");

            entity.HasOne(d => d.Salon).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.SalonId)
                .HasConstraintName("FK_Reviews_Salon");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__UserId__236943A5");
        });

        modelBuilder.Entity<Salon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Salons__3214EC07F442A0F6");

            entity.Property(e => e.AboutSalon).HasMaxLength(1000);
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.ClosingTime).HasMaxLength(10);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.IsVisible).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.OpeningTime).HasMaxLength(10);
            entity.Property(e => e.OwnerId).HasDefaultValue(true);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.BarberOwner).WithMany(p => p.Salons)
                .HasForeignKey(d => d.BarberOwnerId)
                .HasConstraintName("FK_Salons_Barbers");
        });

        modelBuilder.Entity<SalonBarber>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SalonBar__3214EC0797EBFB7C");

            entity.HasIndex(e => e.Email, "UQ__SalonBar__A9D10534E50AC774").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);

            entity.HasOne(d => d.Barber).WithMany(p => p.SalonBarbers)
                .HasForeignKey(d => d.BarberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SalonBarb__Barbe__787EE5A0");

            entity.HasOne(d => d.Salon).WithMany(p => p.SalonBarbers)
                .HasForeignKey(d => d.SalonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SalonBarb__Salon__778AC167");
        });

        modelBuilder.Entity<SalonImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SalonIma__3214EC075BCED9ED");

            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Salon).WithMany(p => p.SalonImages)
                .HasForeignKey(d => d.SalonId)
                .HasConstraintName("FK__SalonImag__Salon__59FA5E80");
        });

        modelBuilder.Entity<SelectedService>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Selected__3214EC077CC93173");

            entity.Property(e => e.BarberName).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ServiceName).HasMaxLength(255);

            entity.HasOne(d => d.Service).WithMany(p => p.SelectedServices)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_SelectedServices_Services");

            entity.HasOne(d => d.User).WithMany(p => p.SelectedServices)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SelectedS__UserI__14270015");
        });

        modelBuilder.Entity<SelectedServiceTemp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Selected__3214EC07935557FD");

            entity.ToTable("SelectedServiceTemp");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ServiceName).HasMaxLength(100);
            entity.Property(e => e.UserId).HasMaxLength(100);

            entity.HasOne(d => d.Service).WithMany(p => p.SelectedServiceTemps)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SelectedServiceTemp_Services");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Services__3214EC072417FDC5");

            entity.Property(e => e.BarberFirstName).HasMaxLength(100);
            entity.Property(e => e.BarberLastName).HasMaxLength(100);
            entity.Property(e => e.Category)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("Not Assigned");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ServiceName).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Barber).WithMany(p => p.Services)
                .HasForeignKey(d => d.BarberId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Service_Barber");

            entity.HasOne(d => d.Salon).WithMany(p => p.Services)
                .HasForeignKey(d => d.SalonId)
                .HasConstraintName("FK__Services__SalonI__6B24EA82");
        });

        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TimeSlot__3214EC071D829A30");

            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.Barber).WithMany(p => p.TimeSlots)
                .HasForeignKey(d => d.BarberId)
                .HasConstraintName("FK_TimeSlots_Barbers");
        });

        modelBuilder.Entity<TrainingCourse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Training__3214EC072FF581E7");

            entity.Property(e => e.CourseName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Salon).WithMany(p => p.TrainingCourses)
                .HasForeignKey(d => d.SalonId)
                .HasConstraintName("FK__TrainingC__Salon__2DE6D218");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC070D6DE38D");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534A2F4F46E").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.ProfileImagePath).HasMaxLength(500);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserType).HasMaxLength(50);
        });

        modelBuilder.Entity<Vacancy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Vacancie__3214EC07C338BF2C");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasDefaultValue("");
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.Requirements)
                .HasMaxLength(1000)
                .HasDefaultValue("");
            entity.Property(e => e.Salary)
                .HasMaxLength(255)
                .HasDefaultValue("");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Salon).WithMany(p => p.Vacancies)
                .HasForeignKey(d => d.SalonId)
                .HasConstraintName("FK__Vacancies__Salon__29221CFB");
        });

        modelBuilder.Entity<WorkingHour>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WorkingH__3214EC071472D23A");

            entity.Property(e => e.DayOfWeek).HasMaxLength(20);

            entity.HasOne(d => d.Salon).WithMany(p => p.WorkingHours)
                .HasForeignKey(d => d.SalonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkingHo__Salon__5629CD9C");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
