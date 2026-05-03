using AutoMapper;
using BLL.DTOs;
using BLL.DTOs.Receptionist;
using BLL.Interfaces;
using Common.Results;
using DAL.Interfaces;

namespace BLL.Services.Implementations;

public class ReceptionistService : IReceptionistService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ReceptionistService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<OperationResult<IEnumerable<ReceptionistDto>>> GetAllAsync()
    {
        try
        {
            var receptionists = await _uow.Receptionists.GetAllWithUsersAsync();
            var dtos = _mapper.Map<IEnumerable<ReceptionistDto>>(receptionists);
            return OperationResult<IEnumerable<ReceptionistDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return OperationResult<IEnumerable<ReceptionistDto>>.Failure(
                $"An error occurred while retrieving receptionists: {ex.Message}");
        }
    }

    public async Task<OperationResult<IEnumerable<ReceptionistDto>>> GetActiveAsync()
    {
        try
        {
            var receptionists = await _uow.Receptionists.GetActiveAsync();
            var dtos = _mapper.Map<IEnumerable<ReceptionistDto>>(receptionists);
            return OperationResult<IEnumerable<ReceptionistDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return OperationResult<IEnumerable<ReceptionistDto>>.Failure(
                $"An error occurred while retrieving active receptionists: {ex.Message}");
        }
    }

    public async Task<OperationResult<ReceptionistDto>> GetByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
                return OperationResult<ReceptionistDto>.Failure("Invalid receptionist id");

            var receptionist = await _uow.Receptionists.GetWithUserAsync(id);
            if (receptionist == null)
                return OperationResult<ReceptionistDto>.Failure("Receptionist not found");

            var dto = _mapper.Map<ReceptionistDto>(receptionist);
            return OperationResult<ReceptionistDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return OperationResult<ReceptionistDto>.Failure(
                $"An error occurred while retrieving the receptionist: {ex.Message}");
        }
    }

    public async Task<OperationResult<ReceptionistDto>> GetByUserIdAsync(string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                return OperationResult<ReceptionistDto>.Failure("User id is required");

            var receptionist = await _uow.Receptionists.GetByUserIdAsync(userId);
            if (receptionist == null)
                return OperationResult<ReceptionistDto>.Failure("Receptionist profile not found");

            var dto = _mapper.Map<ReceptionistDto>(receptionist);
            return OperationResult<ReceptionistDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return OperationResult<ReceptionistDto>.Failure(
                $"An error occurred while retrieving the receptionist: {ex.Message}");
        }
    }

    public async Task<OperationResult> UpdateAsync(UpdateReceptionistDto dto)
    {
        try
        {
            if (dto == null)
                return OperationResult.Failure("Request data is empty");

            if (dto.Id <= 0)
                return OperationResult.Failure("Invalid receptionist id");

            if (string.IsNullOrWhiteSpace(dto.FullName))
                return OperationResult.Failure("Full name is required");

            var receptionist = await _uow.Receptionists.GetWithUserAsync(dto.Id);
            if (receptionist == null)
                return OperationResult.Failure("Receptionist not found");

            receptionist.FullName = dto.FullName.Trim();
            receptionist.IsActive = dto.IsActive;

            if (receptionist.ApplicationUser != null)
            {
                receptionist.ApplicationUser.FullName = dto.FullName.Trim();

                if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                    receptionist.ApplicationUser.PhoneNumber = dto.PhoneNumber.Trim();
            }

            _uow.Receptionists.Update(receptionist);
            await _uow.SaveChangesAsync();

            return OperationResult.Success("Receptionist updated successfully");
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(
                $"An error occurred while updating the receptionist: {ex.Message}");
        }
    }

    public async Task<OperationResult> ToggleActiveAsync(int id)
    {
        try
        {
            if (id <= 0)
                return OperationResult.Failure("Invalid receptionist id");

            var receptionist = await _uow.Receptionists.GetByIdAsync(id);
            if (receptionist == null)
                return OperationResult.Failure("Receptionist not found");

            receptionist.IsActive = !receptionist.IsActive;

            _uow.Receptionists.Update(receptionist);
            await _uow.SaveChangesAsync();

            var status = receptionist.IsActive ? "activated" : "deactivated";
            return OperationResult.Success($"Receptionist {status} successfully");
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(
                $"An error occurred while toggling receptionist status: {ex.Message}");
        }
    }
}