using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace DAL.Repositories
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly ClinicDbContext _context;
     
        public IPatientRepository Patients {  get;private set; }

        public IDoctorRepository Doctors {  get; private set; }

        public IDoctorScheduleRepository DoctorSchedules {  get; private set; }

        public IAppointmentRepository Appointments {  get; private set; }

        public IMedicalRecordRepository MedicalRecords {  get; private set; }
        public IProcedureTypeRepository ProcedureTypes {  get; private set; }

        public IProcedureRepository Procedures {  get; private set; }

        public IInvoiceRepository Invoices {  get; private set; }

        public UnitOfWork(ClinicDbContext context, IPatientRepository patients, IDoctorRepository doctors, IDoctorScheduleRepository doctorSchedules, IAppointmentRepository appointments, IMedicalRecordRepository medicalRecords, IProcedureTypeRepository procedureTypes, IProcedureRepository procedures, IInvoiceRepository invoices)
        {
            _context = context;
            Patients = patients;
            Doctors = doctors;
            DoctorSchedules = doctorSchedules;
            Appointments = appointments;
            MedicalRecords = medicalRecords;
            ProcedureTypes = procedureTypes;
            Procedures = procedures;
            Invoices = invoices;
        }



       
        public async Task<IDbContextTransaction> BeginTransactionAsync() =>
           await _context.Database.BeginTransactionAsync();


        public void Dispose() => _context.Dispose();

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
