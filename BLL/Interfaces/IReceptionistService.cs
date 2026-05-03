using BLL.DTOs.Receptionist;
using Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IReceptionistService
    {
        Task<OperationResult<IEnumerable<ReceptionistDto>>> GetAllAsync();
        Task<OperationResult<IEnumerable<ReceptionistDto>>> GetActiveAsync();
        Task<OperationResult<ReceptionistDto>> GetByIdAsync(int id);
        Task<OperationResult<ReceptionistDto>> GetByUserIdAsync(string userId);
        Task<OperationResult> UpdateAsync(UpdateReceptionistDto dto);
        Task<OperationResult> ToggleActiveAsync(int id);
    }
}
