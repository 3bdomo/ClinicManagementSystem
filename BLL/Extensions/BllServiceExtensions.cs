using BLL.Interfaces;
using BLL.Services;
using BLL.Services.Implementations;
using ClinicSystem.BLL.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.Extensions;

public static class BllServiceExtensions
{
    public static IServiceCollection AddBllServices(this IServiceCollection services)
    {
       
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        services.AddScoped<IPatientService, PatientService>();

        services.AddScoped<IDoctorService, DoctorService>();

        //services.AddScoped<IAppointmentService, AppointmentService>();

        services.AddScoped<IBillingService, BillingService>();


        services.AddScoped<IMedicalRecordService, MedicalRecordService>();

        services.AddScoped<IAuthService, AuthService>();


        services.AddScoped<IMedicalRecordService, MedicalRecordService>();

        services.AddScoped<IDoctorScheduleService, DoctorScheduleService>();

        services.AddScoped<IPatientAccountService, PatientAccountService>();

        //services.AddScoped <IDashboardService, DashboardService>();

        services.AddScoped<IReceptionistService, ReceptionistService>();

        services.AddScoped<IUserService, UserService>();





        return services;
    }
}
