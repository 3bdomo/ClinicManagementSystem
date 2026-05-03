using Microsoft.EntityFrameworkCore.Storage;

namespace DAL.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPatientRepository        Patients        { get; }
    IDoctorRepository         Doctors         { get; }
    IDoctorScheduleRepository DoctorSchedules { get; }
    IAppointmentRepository    Appointments    { get; }
    IMedicalRecordRepository  MedicalRecords  { get; }
    IProcedureTypeRepository  ProcedureTypes  { get; }
    IProcedureRepository      Procedures      { get; }
    IInvoiceRepository        Invoices        { get; }
    IReceptionistRepository   Receptionists   { get; }

    Task<IDbContextTransaction> BeginTransactionAsync();
    Task SaveChangesAsync();
}
