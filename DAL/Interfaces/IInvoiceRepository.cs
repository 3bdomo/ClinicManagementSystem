using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicSystem.DAL.Models;

namespace DAL.Interfaces
{
    public interface IInvoiceRepository: IGenericRepository<Invoice>
    {
        Task<IEnumerable<Invoice>> GetByPatientAsync(int patientId);
        Task<IEnumerable<Invoice>> GetUnpaidAsync();
        Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task<Invoice?> GetWithItemsAsync(int invoiceId);
    }
}
