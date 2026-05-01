using ClinicSystem.DAL.Models;
using Common.Enums;
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
    internal class InvoiceRepository:  GenericRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(ClinicDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Invoice>> GetByPatientAsync(int patientId)
            => await FindAsync(i => i.PatientId == patientId);
        public async Task<IEnumerable<Invoice>> GetUnpaidAsync()
            => await FindAsync(i => i.Status == InvoiceStatus.Unpaid);
        public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to)
            => await FindAsync(i => i.CreatedAt >= from && i.CreatedAt <= to);
        public async Task<Invoice?> GetWithItemsAsync(int invoiceId)
            => await _dbSet.Include(i => i.Items)
                           .FirstOrDefaultAsync(i => i.Id == invoiceId);
    }
}
