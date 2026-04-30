using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using Common.Enums;
namespace DAL.Interfaces
{
    public interface IDoctorRepository: IGenericRepository<Doctor>
    {

        Task<IEnumerable<Doctor>> GetBySpecializationAsync(Specialization spec);
        Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync();
        Task<Doctor?> GetWithUserAsync(int doctorId);
    }
}
