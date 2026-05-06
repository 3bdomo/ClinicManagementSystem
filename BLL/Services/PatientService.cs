using AutoMapper;
using BLL.DTOs.Patient;
using BLL.Interfaces;
using Common.Results;
using DAL.Interfaces;
using ClinicSystem.DAL.Models;

namespace BLL.Services
{
    internal class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PatientService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OperationResult> CreateAsync(PatientDto dto)
        {
            var existingPatient = await _unitOfWork.Patients.GetByNationalIdAsync(dto.NationalId);
            if (existingPatient != null)
                return OperationResult.Failure("A patient with the same National ID already exists.");
            var patientEntity = _mapper.Map<Patient>(dto);
            await _unitOfWork.Patients.AddAsync(patientEntity);
            await _unitOfWork.SaveChangesAsync();
            return OperationResult.Success("Patient created successfully.");


        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(id);
            if (patient == null)
                return OperationResult.Failure("Patient not found.");

            _unitOfWork.Patients.Delete(patient);
            await _unitOfWork.SaveChangesAsync();
            return OperationResult.Success("Patient deleted successfully.");
        }

        public async Task<OperationResult<IEnumerable<PatientDto>>> GetAllAsync()
        {
            //Default to first page with a large page size 
            var patients = await _unitOfWork.Patients.GetAllAsync(1, 5000);
            var patientDtos = _mapper.Map<IEnumerable<PatientDto>>(patients);
            return OperationResult<IEnumerable<PatientDto>>.Success(patientDtos);
        }

        public async Task<OperationResult<IEnumerable<PatientDto>>> GetAllAsync(int pageNumber, int pageSize)
        {
            var patients = await _unitOfWork.Patients.GetAllAsync(pageNumber, pageSize);
            var patientDtos = _mapper.Map<IEnumerable<PatientDto>>(patients);
            return OperationResult<IEnumerable<PatientDto>>.Success(patientDtos);
        }

        public async Task<OperationResult<PatientDto>> GetByIdAsync(int id)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(id);
            if (patient == null)
                return OperationResult<PatientDto>.Failure("Patient not found.");

            var patientDto = _mapper.Map<PatientDto>(patient);
            return OperationResult<PatientDto>.Success(patientDto);
        }

        public async Task<OperationResult<IEnumerable<PatientDto>>> GetDeletedAsync()
        {
            var deletedPatients = await _unitOfWork.Patients.GetDeletedAsync();
            if (deletedPatients == null || !deletedPatients.Any())
                return OperationResult<IEnumerable<PatientDto>>.Failure("No deleted patients found.");

            var deletedPatientDtos = _mapper.Map<IEnumerable<PatientDto>>(deletedPatients);
            return OperationResult<IEnumerable<PatientDto>>.Success(deletedPatientDtos);
        }

        public async Task<OperationResult<PatientHistoryDto>> GetFullHistoryAsync(int id)
        {
            var patient = await _unitOfWork.Patients.GetWithFullHistoryAsync(id);
            if (patient == null)
                return OperationResult<PatientHistoryDto>.Failure("Patient history not found.");

            var patientHistoryDto = _mapper.Map<PatientHistoryDto>(patient);
            return OperationResult<PatientHistoryDto>.Success(patientHistoryDto);
        }

        public async Task<OperationResult> RestoreAsync(int id)
        {
            var restored = await _unitOfWork.Patients.RestoreAsync(id);
            if (!restored)
                return OperationResult.Failure("Patient not found or could not be restored.");

            await _unitOfWork.SaveChangesAsync();
            return OperationResult.Success("Patient restored successfully.");
        }

        public async Task<OperationResult<IEnumerable<PatientDto>>> SearchAsync(string query)
        {
            var patients = await _unitOfWork.Patients.SearchAsync(query);
            if (patients == null || !patients.Any())
                return OperationResult<IEnumerable<PatientDto>>.Failure("No patients found matching the search criteria.");

            var patientDtos = _mapper.Map<IEnumerable<PatientDto>>(patients);
            return OperationResult<IEnumerable<PatientDto>>.Success(patientDtos);
        }

        public async Task<OperationResult> UpdateAsync(PatientDto dto)
        {
            var existingPatient = await _unitOfWork.Patients.GetByIdAsync(dto.Id);
            if (existingPatient == null)
                return OperationResult.Failure("Patient not found.");

            _mapper.Map(dto, existingPatient);
            _unitOfWork.Patients.Update(existingPatient);
            await _unitOfWork.SaveChangesAsync();
            return OperationResult.Success("Patient updated successfully.");
        }

        public async Task<OperationResult<int>> GetPatientIdByApplicationUserIdAsync(string applicationUserId)
        {
            if (string.IsNullOrWhiteSpace(applicationUserId))
                return OperationResult<int>.Failure("Invalid user id.");

            var patient = await _unitOfWork.Patients.GetByUserIdAsync(applicationUserId);

            if (patient == null)
                return OperationResult<int>.Failure("Patient profile not found.");

            return OperationResult<int>.Success(patient.Id);
        }

    }
}