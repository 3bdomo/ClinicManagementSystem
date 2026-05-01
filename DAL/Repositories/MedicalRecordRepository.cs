using ClinicSystem.DAL.Models;
using DAL.Context;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    internal class MedicalRecordRepository : GenericRepository<MedicalRecord>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(ClinicDbContext context) : base(context)
        {
        }

        public async Task<MedicalRecord?> GetByAppointmentAsync(int appointmentId)
            => await _dbSet.FirstOrDefaultAsync(r => r.AppointmentId == appointmentId);

        public async Task<IEnumerable<MedicalRecord>> GetByPatientAsync(int patientId)
            => await _dbSet.Where(r => r.PatientId == patientId).ToListAsync();

        public async Task<MedicalRecord?> GetFullAsync(int recordId)
            => await _dbSet
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                .Include(r => r.Appointment)
                .Include(r => r.Attachments)
                .Include(r => r.Procedures)
                .FirstOrDefaultAsync(r => r.Id == recordId);

        public async Task<IEnumerable<MedicalRecord>> GetUpcomingFollowUpsAsync(DateTime from, DateTime to)
            => await _dbSet
                .Where(r => r.FollowUpDate.HasValue
                         && r.FollowUpDate.Value >= from
                         && r.FollowUpDate.Value <= to)
                .ToListAsync();
    }

}