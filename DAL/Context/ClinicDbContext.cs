using ClinicSystem.DAL.Models;
using Common.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Context;

public class ClinicDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly ICurrentUserService _currentUserService;

    public ClinicDbContext(
        DbContextOptions<ClinicDbContext> options,
        ICurrentUserService currentUserService) : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<Doctor>           Doctors           { get; set; }
    public DbSet<DoctorSchedule>   DoctorSchedules   { get; set; }
    public DbSet<Patient>          Patients          { get; set; }
    public DbSet<Appointment>      Appointments      { get; set; }
    public DbSet<MedicalRecord>    MedicalRecords    { get; set; }
    public DbSet<RecordAttachment> RecordAttachments { get; set; }
    public DbSet<ProcedureType>    ProcedureTypes    { get; set; }
    public DbSet<Procedure>        Procedures        { get; set; }
    public DbSet<Invoice>          Invoices          { get; set; }
    public DbSet<InvoiceItem>      InvoiceItems      { get; set; }
    public DbSet<Receptionist>     Receptionists     { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClinicDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId ?? "System";
        var now    = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            bool isSoftDeleting = entry.Entity is ISoftDeletable sd
                                  && sd.IsDeleted
                                  && entry.OriginalValues.GetValue<bool>("IsDeleted") == false;

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = userId;
            }

            if (!isSoftDeleting &&
                entry.State is EntityState.Added or EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = userId;
            }
        }

        foreach (var entry in ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State             = EntityState.Modified;
                entry.Entity.IsDeleted  = true;
                entry.Entity.DeletedAt  = now;
                entry.Entity.DeletedBy  = userId;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
