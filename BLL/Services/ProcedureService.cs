using AutoMapper;
using BLL.DTOs.Procedure;
using BLL.Interfaces;
using ClinicSystem.DAL.Models;
using Common.Results;
using DAL.Interfaces;

namespace BLL.Services
{
    public class ProcedureService : IProcedureService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProcedureService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

       

        public async Task<OperationResult<IEnumerable<ProcedureDto>>> GetAllAsync()
        {
            var procedures = await _unitOfWork.Procedures.GetAllAsync(1, int.MaxValue);
            return OperationResult<IEnumerable<ProcedureDto>>.Success(_mapper.Map<IEnumerable<ProcedureDto>>(procedures));
        }

        public async Task<OperationResult<ProcedureDto>> GetByIdAsync(int id)
        {
            var procedure = await _unitOfWork.Procedures.GetByIdAsync(id);
            if (procedure is null)
                return OperationResult<ProcedureDto>.Failure("Procedure not found.");

            return OperationResult<ProcedureDto>.Success(_mapper.Map<ProcedureDto>(procedure));
        }

        public async Task<OperationResult<int>> CreateAsync(CreateProcedureDto dto)
        {
            var medicalRecord = await _unitOfWork.MedicalRecords.GetByIdAsync(dto.MedicalRecordId);
            if (medicalRecord is null)
                return OperationResult<int>.Failure("Medical record not found.");

            var procedureType = await _unitOfWork.ProcedureTypes.GetByIdAsync(dto.ProcedureTypeId);
            if (procedureType is null)
                return OperationResult<int>.Failure("Procedure type not found.");

            if (!procedureType.IsActive)
                return OperationResult<int>.Failure("The selected procedure type is not active.");

            var procedure = _mapper.Map<Procedure>(dto);

            if (procedure.Cost == 0)
                procedure.Cost = procedureType.DefaultCost;

            await _unitOfWork.Procedures.AddAsync(procedure);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult<int>.Success(procedure.Id, "Procedure created successfully.");
        }

        public async Task<OperationResult> UpdateAsync(UpdateProcedureDto dto)
        {
            var procedure = await _unitOfWork.Procedures.GetByIdAsync(dto.Id);
            if (procedure is null)
                return OperationResult.Failure("Procedure not found.");

            if (dto.DurationMinutes.HasValue)
                procedure.DurationMinutes = dto.DurationMinutes;

            if (dto.Notes is not null)
                procedure.Notes = dto.Notes;

            if (dto.AfterCareNotes is not null)
                procedure.AfterCareNotes = dto.AfterCareNotes;

            if (dto.Cost.HasValue)
                procedure.Cost = dto.Cost.Value;

            _unitOfWork.Procedures.Update(procedure);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success("Procedure updated successfully.");
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            var procedure = await _unitOfWork.Procedures.GetByIdAsync(id);
            if (procedure is null)
                return OperationResult.Failure("Procedure not found.");

            _unitOfWork.Procedures.Delete(procedure);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success("Procedure deleted successfully.");
        }

        public async Task<OperationResult<IEnumerable<ProcedureDto>>> GetByMedicalRecordAsync(int medicalRecordId)
        {
            var record = await _unitOfWork.MedicalRecords.GetByIdAsync(medicalRecordId);
            if (record is null)
                return OperationResult<IEnumerable<ProcedureDto>>.Failure("Medical record not found.");

            var procedures = await _unitOfWork.Procedures.GetByMedicalRecordAsync(medicalRecordId);
            return OperationResult<IEnumerable<ProcedureDto>>.Success(_mapper.Map<IEnumerable<ProcedureDto>>(procedures));
        }

        public async Task<OperationResult<IEnumerable<ProcedureDto>>> GetByPatientAsync(int patientId)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(patientId);
            if (patient is null)
                return OperationResult<IEnumerable<ProcedureDto>>.Failure("Patient not found.");

            var procedures = await _unitOfWork.Procedures.GetByPatientAsync(patientId);
            return OperationResult<IEnumerable<ProcedureDto>>.Success(_mapper.Map<IEnumerable<ProcedureDto>>(procedures));
        }

        

        public async Task<OperationResult<IEnumerable<ProcedureTypeDto>>> GetAllTypesAsync()
        {
            var types = await _unitOfWork.ProcedureTypes.GetAllAsync(1, int.MaxValue);
            return OperationResult<IEnumerable<ProcedureTypeDto>>.Success(_mapper.Map<IEnumerable<ProcedureTypeDto>>(types));
        }

        public async Task<OperationResult<IEnumerable<ProcedureTypeDto>>> GetActiveTypesAsync()
        {
            var types = await _unitOfWork.ProcedureTypes.GetActiveAsync();
            return OperationResult<IEnumerable<ProcedureTypeDto>>.Success(_mapper.Map<IEnumerable<ProcedureTypeDto>>(types));
        }

        public async Task<OperationResult<ProcedureTypeDto>> GetTypeByIdAsync(int id)
        {
            var type = await _unitOfWork.ProcedureTypes.GetByIdAsync(id);
            if (type is null)
                return OperationResult<ProcedureTypeDto>.Failure("Procedure type not found.");

            return OperationResult<ProcedureTypeDto>.Success(_mapper.Map<ProcedureTypeDto>(type));
        }

        public async Task<OperationResult<int>> CreateTypeAsync(CreateProcedureTypeDto dto)
        {
            var existing = await _unitOfWork.ProcedureTypes.GetByNameAsync(dto.Name);
            if (existing != null)
                return OperationResult<int>.Failure($"A procedure type with name '{dto.Name}' already exists.");

            var procedureType = _mapper.Map<ProcedureType>(dto);
            await _unitOfWork.ProcedureTypes.AddAsync(procedureType);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult<int>.Success(procedureType.Id, "Procedure type created successfully.");
        }

        public async Task<OperationResult> UpdateTypeAsync(UpdateProcedureTypeDto dto)
        {
            var procedureType = await _unitOfWork.ProcedureTypes.GetByIdAsync(dto.Id);
            if (procedureType is null)
                return OperationResult.Failure("Procedure type not found.");

            if (dto.Name is not null)
                procedureType.Name = dto.Name;

            if (dto.Description is not null)
                procedureType.Description = dto.Description;

            if (dto.DefaultCost.HasValue)
                procedureType.DefaultCost = dto.DefaultCost.Value;

            if (dto.IsActive.HasValue)
                procedureType.IsActive = dto.IsActive.Value;

            _unitOfWork.ProcedureTypes.Update(procedureType);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success("Procedure type updated successfully.");
        }

        public async Task<OperationResult> DeactivateTypeAsync(int id)
        {
            var procedureType = await _unitOfWork.ProcedureTypes.GetByIdAsync(id);
            if (procedureType is null)
                return OperationResult.Failure("Procedure type not found.");

            if (!procedureType.IsActive)
                return OperationResult.Failure("Procedure type is already inactive.");

            procedureType.IsActive = false;
            _unitOfWork.ProcedureTypes.Update(procedureType);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success("Procedure type deactivated successfully.");
        }

        public async Task<OperationResult> ActivateTypeAsync(int id)
        {
            var procedureType = await _unitOfWork.ProcedureTypes.GetByIdAsync(id);
            if (procedureType is null)
                return OperationResult.Failure("Procedure type not found.");

            if (procedureType.IsActive)
                return OperationResult.Failure("Procedure type is already active.");

            procedureType.IsActive = true;
            _unitOfWork.ProcedureTypes.Update(procedureType);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success("Procedure type activated successfully.");
        }
    }
}