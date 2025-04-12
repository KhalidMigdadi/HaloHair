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

    public virtual DbSet<LoyaltyPoint> LoyaltyPoints { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<Salon> Salons { get; set; }

    public virtual DbSet<SalonBarber> SalonBarbers { get; set; }

    public virtual DbSet<SalonImage> SalonImages { get; set; }

    public virtual DbSet<SelectedService> SelectedServices { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

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
            entity.HasKey(e => e.Id).HasName("PK__Appointm__3214EC07488C0395");

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
                .HasConstraintName("FK__Appointme__Salon__52593CB8");

            entity.HasOne(d => d.Service).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__Appointme__Servi__534D60F1");

            entity.HasOne(d => d.User).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Appointme__UserI__5165187F");
        });

        modelBuilder.Entity<AppointmentService>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Appointm__3214EC07BDA16126");

            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ServiceName).HasMaxLength(255);

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentServices)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Appoi__46B27FE2");

            entity.HasOne(d => d.Service).WithMany(p => p.AppointmentServices)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__Appointme__Servi__47A6A41B");
        });

        modelBuilder.Entity<Barber>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Barbers__3214EC07035E9CAD");

            entity.HasIndex(e => e.Email, "UQ__Barbers__A9D10534F09A8F82").IsUnique();

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
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Barbers_Salons");
        });

        modelBuilder.Entity<BarberService>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BarberSe__3214EC0780796B90");

            entity.Property(e => e.BarberFirstName).HasMaxLength(100);
            entity.Property(e => e.BarberLastName).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SalonName).HasMaxLength(100);

            entity.HasOne(d => d.Barber).WithMany(p => p.BarberServices)
                .HasForeignKey(d => d.BarberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BarberSer__Barbe__4D94879B");

            entity.HasOne(d => d.Service).WithMany(p => p.BarberServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BarberSer__Servi__4E88ABD4");
        });

        modelBuilder.Entity<BarberWorkImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BarberWo__3214EC07C4005F4B");

            entity.Property(e => e.ImageUrl).HasMaxLength(255);

            entity.HasOne(d => d.Barber).WithMany(p => p.BarberWorkImages)
                .HasForeignKey(d => d.BarberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BarberWorkImages_BarberId");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bookings__3214EC0735E7F556");

            entity.HasOne(d => d.Barber).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.BarberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_Barbers");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_Users");
        });

        modelBuilder.Entity<BookingService>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookingS__3214EC07A4ACB580");

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
            entity.HasKey(e => e.Id).HasName("PK__BookingT__3214EC07291D01D7");

            entity.ToTable("BookingTemp");

            entity.Property(e => e.AppointmentTime).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ServiceName).HasMaxLength(255);
            entity.Property(e => e.UserId).HasMaxLength(450);
        });

        modelBuilder.Entity<BookingsHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bookings__3214EC0752554634");

            entity.ToTable("BookingsHistory");

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
            entity.HasKey(e => e.Id).HasName("PK__Favorite__3214EC070A890E84");

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

        modelBuilder.Entity<LoyaltyPoint>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LoyaltyP__3214EC07A56C1F5B");

            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Points).HasDefaultValue(0);

            entity.HasOne(d => d.User).WithMany(p => p.LoyaltyPoints)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__LoyaltyPo__UserI__5DCAEF64");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Promotio__3214EC07F618AE89");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Discount).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.PromotionTitle).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ValidUntil).HasColumnType("datetime");

            entity.HasOne(d => d.Salon).WithMany(p => p.Promotions)
                .HasForeignKey(d => d.SalonId)
                .HasConstraintName("FK__Promotion__Salon__70DDC3D8");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reviews__3214EC0786B0ABA6");

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
                .HasConstraintName("FK__Reviews__Booking__681373AD");

            entity.HasOne(d => d.Salon).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.SalonId)
                .HasConstraintName("FK_Reviews_Salon");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__UserId__671F4F74");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sales__3214EC077065D1DD");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SaleDate).HasColumnType("datetime");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Salon).WithMany(p => p.Sales)
                .HasForeignKey(d => d.SalonId)
                .HasConstraintName("FK__Sales__SalonId__7A672E12");
        });

        modelBuilder.Entity<Salon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Salons__3214EC072FC7AB3C");

            entity.Property(e => e.AboutSalon).HasMaxLength(1000);
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.ClosingTime).HasMaxLength(10);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.OpeningTime).HasMaxLength(10);
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
            entity.HasKey(e => e.Id).HasName("PK__SalonBar__3214EC07F39DF998");

            entity.HasIndex(e => e.Email, "UQ__SalonBar__A9D105343D8095A6").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);

            entity.HasOne(d => d.Barber).WithMany(p => p.SalonBarbers)
                .HasForeignKey(d => d.BarberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SalonBarb__Barbe__03F0984C");

            entity.HasOne(d => d.Salon).WithMany(p => p.SalonBarbers)
                .HasForeignKey(d => d.SalonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SalonBarb__Salon__02FC7413");
        });

        modelBuilder.Entity<SalonImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SalonIma__3214EC072846D742");

            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Salon).WithMany(p => p.SalonImages)
                .HasForeignKey(d => d.SalonId)
                .HasConstraintName("FK__SalonImag__Salon__0E6E26BF");
        });

        modelBuilder.Entity<SelectedService>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Selected__3214EC07D8BA9248");

            entity.Property(e => e.BarberName).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ServiceName).HasMaxLength(255);

            entity.HasOne(d => d.Service).WithMany(p => p.SelectedServices)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_SelectedServices_Services");

            entity.HasOne(d => d.User).WithMany(p => p.SelectedServices)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SelectedS__UserI__236943A5");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Services__3214EC07F3625981");

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
                .HasConstraintName("FK__Services__SalonI__48CFD27E");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC07B3934CDB");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.SubscriptionType).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Subscript__UserI__628FA481");
        });

        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TimeSlot__3214EC076F622FCB");

            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.Barber).WithMany(p => p.TimeSlots)
                .HasForeignKey(d => d.BarberId)
                .HasConstraintName("FK_TimeSlots_Barbers");
        });

        modelBuilder.Entity<TrainingCourse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Training__3214EC07E478EC15");

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
                .HasConstraintName("FK__TrainingC__Salon__6C190EBB");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07AA937CEB");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105340181565F").IsUnique();

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
            entity.HasKey(e => e.Id).HasName("PK__Vacancie__3214EC07094CB76A");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Salon).WithMany(p => p.Vacancies)
                .HasForeignKey(d => d.SalonId)
                .HasConstraintName("FK__Vacancies__Salon__6754599E");
        });

        modelBuilder.Entity<WorkingHour>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WorkingH__3214EC07D3DEF159");

            entity.Property(e => e.DayOfWeek).HasMaxLength(20);

            entity.HasOne(d => d.Salon).WithMany(p => p.WorkingHours)
                .HasForeignKey(d => d.SalonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkingHo__Salon__123EB7A3");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
