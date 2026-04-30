using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
namespace DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IPatientRepository Patients {get; }
        IDoctorRepository Doctors {get; }
        IDoctorScheduleRepository DoctorSchedules {get; }
        IAppointmentRepository Appointments {get; }
        IMedicalRecordRepository MedicalRecords {get; }
        IProcedureTypeRepository ProcedureTypes {get; }
        IProcedureRepository Procedures {get; }
        IInvoiceRepository Invoices {get; }
        Task SaveChangesAsync();    
    }
}
