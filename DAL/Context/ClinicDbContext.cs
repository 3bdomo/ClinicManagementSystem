using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using ClinicSystem.DAL.Models;
using Common.Interfaces;

namespace DAL.Context
{
    public class ClinicDbContext : IdentityDbContext<ApplicationUser>

    {
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ClinicDbContext(
            DbContextOptions<ClinicDbContext> options,
            IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<ProcedureType> ProcedureTypes { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<RecordAttachment> RecordAttachments { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
<<<<<<< HEAD
        public DbSet<Receptionist> Receptionists { get; set; }
=======
        public DbSet<Receptionist> Receptionists { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
>>>>>>> 0426ac091cf4583972dc0e09fa966683424a3a5a

      

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ClinicDbContext).Assembly);

        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)

        {
            var currentUser = _httpContextAccessor
                                  .HttpContext?.User
                                  .FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? "System";

            foreach (var entry in ChangeTracker.Entries<IAuditable>())

            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = currentUser;
                }

                if (entry.State is EntityState.Added or EntityState.Modified)

                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = currentUser;
                }
            }

      
            foreach (var entry in ChangeTracker.Entries<ISoftDeletable>())
            {
                if (entry.State == EntityState.Deleted)

                {
                    entry.State = EntityState.Modified;
 

                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    entry.Entity.DeletedBy = currentUser;

                }
            }

            return await base.SaveChangesAsync(cancellationToken);
  
        }
    }
}