using DAL.Context;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DAL.Extensions;

public static class DalServiceExtensions
{
    public static IServiceCollection AddDalServices(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<ClinicDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IPatientRepository,        PatientRepository>();
        services.AddScoped<IDoctorRepository,         DoctorRepository>();
        services.AddScoped<IDoctorScheduleRepository, DoctorScheduleRepository>();
        services.AddScoped<IAppointmentRepository,    AppointmentRepository>();
        services.AddScoped<IMedicalRecordRepository,  MedicalRecordRepository>();
        services.AddScoped<IProcedureTypeRepository,  ProcedureTypeRepository>();
        services.AddScoped<IProcedureRepository,      ProcedureRepository>();
        services.AddScoped<IInvoiceRepository,        InvoiceRepository>();
        services.AddScoped<IReceptionistRepository,   ReceptionistRepository>();
        services.AddScoped<IUserRepository,            UserRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
