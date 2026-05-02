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
using ClinicSystem.DAL.Models;
namespace BLL.Services
{
    internal class PatientService : IPatientService
    {
      
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PatientService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;//context of database
            _mapper = mapper;
        }
     

        public async Task<OperationResult> CreateAsync(PatientDto dto)
        {
           var patientEntity = _mapper.Map<Patient>(dto);

           await _unitOfWork.Patients.AddAsync(patientEntity);
           await _unitOfWork.SaveChangesAsync();
           return OperationResult.Success("Patient created successfully.");
            //return OperationResult<PatientDto>.Success(_mapper.Map<PatientDto>(patient));

        }

        public Task<OperationResult> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<IEnumerable<PatientDto>>> GetAllAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<PatientDto>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<IEnumerable<PatientDto>>> GetDeletedAsync()
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

        public Task<OperationResult<IEnumerable<PatientDto>>> SearchAsync(string query)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> UpdateAsync(PatientDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
