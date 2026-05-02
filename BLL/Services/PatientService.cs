using AutoMapper;
using BLL.DTOs.Patient;
using BLL.Interfaces;
using Common.Results;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    internal class PatientService : IPatientService
    {
      
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PatientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<OperationResult> CreateAsync(PatientDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<IEnumerable<PatientDTO>>> GetAllAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<PatientDTO>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<IEnumerable<PatientDTO>>> GetDeletedAsync()
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<PatientHistoryDto>> GetFullHistoryAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> RestoreAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<IEnumerable<PatientDTO>>> SearchAsync(string query)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> UpdateAsync(PatientDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
