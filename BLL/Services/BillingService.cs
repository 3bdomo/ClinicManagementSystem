using AutoMapper;
using BLL.DTOs.Billing;
using BLL.Interfaces;
using ClinicSystem.DAL.Models;
using Common.Enums;
using Common.Results;
using DAL.Interfaces;

namespace BLL.Services
{
    public class BillingService : IBillingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BillingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OperationResult<IEnumerable<InvoiceDto>>> GetAllAsync()
        {
            var invoices = await _unitOfWork.Invoices.GetAllAsync(1, int.MaxValue);
            var dtos = _mapper.Map<IEnumerable<InvoiceDto>>(invoices);
            return OperationResult<IEnumerable<InvoiceDto>>.Success(dtos);
        }

        public async Task<OperationResult<IEnumerable<InvoiceDto>>> GetAllAsync(int pageNumber, int pageSize)
        {
            var invoices = await _unitOfWork.Invoices.GetAllAsync(pageNumber, pageSize);
            var dtos = _mapper.Map<IEnumerable<InvoiceDto>>(invoices);
            return OperationResult<IEnumerable<InvoiceDto>>.Success(dtos);
        }

        public async Task<OperationResult<InvoiceDto>> GetByIdAsync(int id)
        {
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(id);
            if (invoice is null)
                return OperationResult<InvoiceDto>.Failure("Invoice not found.");

            var dto = _mapper.Map<InvoiceDto>(invoice);
            await EnrichAuditInfoAsync(dto);
            return OperationResult<InvoiceDto>.Success(dto);
        }

        public async Task<OperationResult<InvoiceDto>> GetWithItemsAsync(int id)
        {
            var invoice = await _unitOfWork.Invoices.GetWithItemsAsync(id);
            if (invoice is null)
                return OperationResult<InvoiceDto>.Failure("Invoice not found.");

            var dto = _mapper.Map<InvoiceDto>(invoice);
            await EnrichAuditInfoAsync(dto);
            return OperationResult<InvoiceDto>.Success(dto);
        }

        public async Task<OperationResult<int>> CreateAsync(CreateInvoiceDto dto)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(dto.PatientId);
            if (patient is null)
                return OperationResult<int>.Failure("Patient not found.");

            var appointment = await _unitOfWork.Appointments.GetFullAsync(dto.AppointmentId);
            if (appointment is null)
                return OperationResult<int>.Failure("Appointment not found.");
            
            if (appointment.Invoice != null)
                return OperationResult<int>.Failure("An invoice already exists for this appointment.");

            if (!dto.Items.Any())
                return OperationResult<int>.Failure("Invoice must have at least one item.");

            var items = _mapper.Map<List<InvoiceItem>>(dto.Items);
            var totalAmount = items.Sum(i => i.Quantity * i.UnitPrice);

            var invoice = new Invoice
            {
                PatientId = dto.PatientId,
                AppointmentId = dto.AppointmentId,
                TotalAmount = totalAmount,
                Status = InvoiceStatus.Unpaid,
                Items = items
            };

            await _unitOfWork.Invoices.AddAsync(invoice);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult<int>.Success(invoice.Id, "Invoice created successfully.");
        }

        public async Task<OperationResult> UpdateAsync(UpdateInvoiceDto dto)
        {
            var invoice = await _unitOfWork.Invoices.GetWithItemsAsync(dto.Id);
            if (invoice is null)
                return OperationResult.Failure("Invoice not found.");

            if (invoice.Status == InvoiceStatus.Paid)
                return OperationResult.Failure("Cannot update a paid invoice.");

            if (dto.Items is not null && dto.Items.Any())
            {
                invoice.Items.Clear();
                var newItems = _mapper.Map<List<InvoiceItem>>(dto.Items);
                foreach (var item in newItems)
                {
                    item.InvoiceId = invoice.Id;
                    invoice.Items.Add(item);
                }
                invoice.TotalAmount = invoice.Items.Sum(i => i.Quantity * i.UnitPrice);
            }

            _unitOfWork.Invoices.Update(invoice);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success("Invoice updated successfully.");
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(id);
            if (invoice is null)
                return OperationResult.Failure("Invoice not found.");

            if (invoice.Status == InvoiceStatus.Paid)
                return OperationResult.Failure("Cannot delete a paid invoice.");

            _unitOfWork.Invoices.Delete(invoice);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success("Invoice deleted successfully.");
        }

        public async Task<OperationResult<IEnumerable<InvoiceDto>>> GetByPatientAsync(int patientId)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(patientId);
            if (patient is null)
                return OperationResult<IEnumerable<InvoiceDto>>.Failure("Patient not found.");

            var invoices = await _unitOfWork.Invoices.GetByPatientAsync(patientId);
            return OperationResult<IEnumerable<InvoiceDto>>.Success(_mapper.Map<IEnumerable<InvoiceDto>>(invoices));
        }

        public async Task<OperationResult<IEnumerable<InvoiceDto>>> GetUnpaidAsync()
        {
            var invoices = await _unitOfWork.Invoices.GetUnpaidAsync();
            return OperationResult<IEnumerable<InvoiceDto>>.Success(_mapper.Map<IEnumerable<InvoiceDto>>(invoices));
        }

        public async Task<OperationResult<IEnumerable<InvoiceDto>>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            if (from > to)
                return OperationResult<IEnumerable<InvoiceDto>>.Failure("Start date must be before end date.");

            var invoices = await _unitOfWork.Invoices.GetByDateRangeAsync(from, to);
            return OperationResult<IEnumerable<InvoiceDto>>.Success(_mapper.Map<IEnumerable<InvoiceDto>>(invoices));
        }

        public async Task<OperationResult> MarkAsPaidAsync(int invoiceId)
            => await ChangeStatusAsync(invoiceId, InvoiceStatus.Paid);

        public async Task<OperationResult> MarkAsPartiallyPaidAsync(int invoiceId)
            => await ChangeStatusAsync(invoiceId, InvoiceStatus.PartiallyPaid);

        public async Task<OperationResult> ChangeStatusAsync(int invoiceId, InvoiceStatus newStatus)
        {
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId);
            if (invoice is null)
                return OperationResult.Failure("Invoice not found.");

            if (invoice.Status == newStatus)
                return OperationResult.Failure($"Invoice is already {newStatus}.");

            invoice.Status = newStatus;

            if (newStatus == InvoiceStatus.Paid)
                invoice.PaidAt = DateTime.UtcNow;
            else
                invoice.PaidAt = null;

            _unitOfWork.Invoices.Update(invoice);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success($"Invoice status changed to {newStatus} successfully.");
        }

        public async Task<OperationResult<BillingStatisticsDto>> GetStatisticsAsync()
        {
            var allInvoices = (await _unitOfWork.Invoices.GetAllAsync(1, int.MaxValue)).ToList();
            return OperationResult<BillingStatisticsDto>.Success(BuildStatistics(allInvoices));
        }

        public async Task<OperationResult<BillingStatisticsDto>> GetStatisticsByDateRangeAsync(DateTime from, DateTime to)
        {
            if (from > to)
                return OperationResult<BillingStatisticsDto>.Failure("Start date must be before end date.");

            var invoices = (await _unitOfWork.Invoices.GetByDateRangeAsync(from, to)).ToList();
            return OperationResult<BillingStatisticsDto>.Success(BuildStatistics(invoices));
        }

        public async Task<OperationResult<decimal>> GetTotalRevenueAsync()
        {
            var allInvoices = await _unitOfWork.Invoices.GetAllAsync(1, int.MaxValue);
            var totalRevenue = allInvoices
                .Where(i => i.Status == InvoiceStatus.Paid)
                .Sum(i => i.TotalAmount);

            return OperationResult<decimal>.Success(totalRevenue);
        }

        private async Task EnrichAuditInfoAsync(InvoiceDto dto)
        {
            if (dto is null) return;

            if (!string.IsNullOrEmpty(dto.CreatedBy))
                dto.CreatedBy = await _unitOfWork.Users.GetFullNameAsync(dto.CreatedBy);

            if (!string.IsNullOrEmpty(dto.UpdatedBy))
                dto.UpdatedBy = await _unitOfWork.Users.GetFullNameAsync(dto.UpdatedBy);
        }

        private static BillingStatisticsDto BuildStatistics(List<Invoice> invoices)
        {
            return new BillingStatisticsDto
            {
                TotalInvoices = invoices.Count,
                TotalRevenue = invoices.Sum(i => i.TotalAmount),
                PaidRevenue = invoices.Where(i => i.Status == InvoiceStatus.Paid).Sum(i => i.TotalAmount),
                UnpaidRevenue = invoices.Where(i => i.Status == InvoiceStatus.Unpaid).Sum(i => i.TotalAmount),
                PartiallyPaidRevenue = invoices.Where(i => i.Status == InvoiceStatus.PartiallyPaid).Sum(i => i.TotalAmount),
                PaidInvoices = invoices.Count(i => i.Status == InvoiceStatus.Paid),
                UnpaidInvoices = invoices.Count(i => i.Status == InvoiceStatus.Unpaid),
                PartiallyPaidInvoices = invoices.Count(i => i.Status == InvoiceStatus.PartiallyPaid)
            };
        }
    }
}