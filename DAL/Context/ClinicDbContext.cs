using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace DAL.Context
{
    public class ClinicDbContext : IdentityDbContext<ApplicationUser>
    {
        public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options)
        {
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





        }
}
