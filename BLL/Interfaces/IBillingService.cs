using BLL.DTOs.Billing;
using Common.Enums;
using Common.Results;

namespace BLL.Interfaces
{
    public interface IBillingService
    {
        Task<OperationResult<IEnumerable<InvoiceDto>>> GetAllAsync();
        Task<OperationResult<IEnumerable<InvoiceDto>>> GetAllAsync(int pageNumber, int pageSize);
        Task<OperationResult<InvoiceDto>> GetByIdAsync(int id);
        Task<OperationResult<InvoiceDto>> GetWithItemsAsync(int id);
        Task<OperationResult<int>> CreateAsync(CreateInvoiceDto dto);
        Task<OperationResult> UpdateAsync(UpdateInvoiceDto dto);
        Task<OperationResult> DeleteAsync(int id);
 
        Task<OperationResult<IEnumerable<InvoiceDto>>> GetByPatientAsync(int patientId);
        Task<OperationResult<IEnumerable<InvoiceDto>>> GetUnpaidAsync();
        Task<OperationResult<IEnumerable<InvoiceDto>>> GetByDateRangeAsync(DateTime from, DateTime to);
 
        Task<OperationResult> MarkAsPaidAsync(int invoiceId);
        Task<OperationResult> MarkAsPartiallyPaidAsync(int invoiceId);
        Task<OperationResult> ChangeStatusAsync(int invoiceId, InvoiceStatus newStatus);
 
        Task<OperationResult<BillingStatisticsDto>> GetStatisticsAsync();
        Task<OperationResult<BillingStatisticsDto>> GetStatisticsByDateRangeAsync(DateTime from, DateTime to);
        Task<OperationResult<decimal>> GetTotalRevenueAsync();
    }
}
