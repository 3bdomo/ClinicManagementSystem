using AutoMapper;
using BLL.DTOs.Receptionist;
using BLL.Interfaces;
using Common.Results;
using DAL.Interfaces;

namespace BLL.Services.Implementations;

public class ReceptionistService : IReceptionistService
{
    private const int MinNameLength = 3;
    private const int MaxNameLength = 100;

    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ReceptionistService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<OperationResult<IEnumerable<ReceptionistDto>>> GetAllAsync()
    {
        var receptionists = await _uow.Receptionists.GetAllWithUsersAsync();
        var dtos = _mapper.Map<List<ReceptionistDto>>(receptionists);

        var message = dtos.Count == 0
            ? "No receptionists found in the system"
            : $"Retrieved {dtos.Count} receptionist(s) successfully";

        return OperationResult<IEnumerable<ReceptionistDto>>.Success(dtos, message);
    }

    public async Task<OperationResult<IEnumerable<ReceptionistDto>>> GetActiveAsync()
    {
        var receptionists = await _uow.Receptionists.GetActiveAsync();
        var dtos = _mapper.Map<List<ReceptionistDto>>(receptionists);

        var message = dtos.Count == 0
            ? "No active receptionists found"
            : $"Retrieved {dtos.Count} active receptionist(s) successfully";

        return OperationResult<IEnumerable<ReceptionistDto>>.Success(dtos, message);
    }

    public async Task<OperationResult<ReceptionistDto>> GetByIdAsync(int id)
    {
        if (id <= 0)
            return OperationResult<ReceptionistDto>.Failure(
                "Receptionist id must be a positive number");

        var receptionist = await _uow.Receptionists.GetWithUserAsync(id);

        if (receptionist == null)
            return OperationResult<ReceptionistDto>.Failure(
                $"Receptionist with id {id} was not found");

        var dto = _mapper.Map<ReceptionistDto>(receptionist);
        return OperationResult<ReceptionistDto>.Success(
            dto,
            "Receptionist retrieved successfully");
    }

    public async Task<OperationResult<ReceptionistDto>> GetByUserIdAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return OperationResult<ReceptionistDto>.Failure("User id is required");

        var receptionist = await _uow.Receptionists.GetByUserIdAsync(userId);

        if (receptionist == null)
            return OperationResult<ReceptionistDto>.Failure(
                "No receptionist profile is linked to this user account");

        var dto = _mapper.Map<ReceptionistDto>(receptionist);
        return OperationResult<ReceptionistDto>.Success(
            dto,
            "Receptionist profile retrieved successfully");
    }

    public async Task<OperationResult> UpdateAsync(UpdateReceptionistDto dto)
    {
        if (dto == null)
            return OperationResult.Failure("Update data is required");

        if (dto.Id <= 0)
            return OperationResult.Failure("Receptionist id must be a positive number");

        if (string.IsNullOrWhiteSpace(dto.FullName))
            return OperationResult.Failure("Full name is required");

        var trimmedName = dto.FullName.Trim();

        if (trimmedName.Length < MinNameLength)
            return OperationResult.Failure(
                $"Full name must be at least {MinNameLength} characters long");

        if (trimmedName.Length > MaxNameLength)
            return OperationResult.Failure(
                $"Full name must not exceed {MaxNameLength} characters");

        var receptionist = await _uow.Receptionists.GetWithUserAsync(dto.Id);

        if (receptionist == null)
            return OperationResult.Failure(
                $"Receptionist with id {dto.Id} was not found");

        receptionist.FullName = trimmedName;
        receptionist.IsActive = dto.IsActive;

        receptionist.ApplicationUser!.FullName = trimmedName;
        receptionist.ApplicationUser.PhoneNumber =
            string.IsNullOrWhiteSpace(dto.PhoneNumber) ? null : dto.PhoneNumber.Trim();

        _uow.Receptionists.Update(receptionist);
        await _uow.SaveChangesAsync();

        return OperationResult.Success("Receptionist updated successfully");
    }

    public async Task<OperationResult> ToggleActiveAsync(int id)
    {
        if (id <= 0)
            return OperationResult.Failure("Receptionist id must be a positive number");

        var receptionist = await _uow.Receptionists.GetByIdAsync(id);

        if (receptionist == null)
            return OperationResult.Failure(
                $"Receptionist with id {id} was not found");

        receptionist.IsActive = !receptionist.IsActive;

        _uow.Receptionists.Update(receptionist);
        await _uow.SaveChangesAsync();

        var status = receptionist.IsActive ? "activated" : "deactivated";
        return OperationResult.Success($"Receptionist has been {status} successfully");
    }
}