using BLL.Interfaces;
using BLL.Services;
using ClinicSystem.BLL.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.Extensions;

public static class BllServiceExtensions
{
    public static IServiceCollection AddBllServices(this IServiceCollection services)
    {
       
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        services.AddScoped<IPatientService, PatientService>();

        return services;
    }
}
