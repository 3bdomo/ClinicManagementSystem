using AutoMapper;
using BLL.DTOs.Appointment;
using BLL.DTOs.Patient;
using BLL.DTOs.Shared;
using BLL.DTOs.User;
using Web.ViewModel;

namespace Web.Mapping;

public class WebMappingProfile : Profile
{
    public WebMappingProfile()
    {

        CreateMap<UserDto, UserRowViewModel>();


        CreateMap<CreateUserViewModel, CreateUserDto>();

  
        CreateMap<UserDto, EditUserViewModel>();

     
        CreateMap<EditUserViewModel, UpdateUserDto>();

        
  
        CreateMap<PatientDto, PatientRowViewModel>()
            .ForMember(dest => dest.HasPortalAccount, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted,        opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt,        opt => opt.Ignore());


        CreateMap<PatientFormViewModel, PatientDto>();
        CreateMap<PatientDto, PatientFormViewModel>();

       
        CreateMap<PatientHistoryDto, PatientDetailsViewModel>()
            .ForMember(dest => dest.Patient,        opt => opt.Ignore())
            .ForMember(dest => dest.Appointments,   opt => opt.Ignore())
            .ForMember(dest => dest.MedicalRecords, opt => opt.Ignore())
            .ForMember(dest => dest.Invoices,       opt => opt.Ignore())
            .ForMember(dest => dest.AuditInfo,      opt => opt.Ignore());


        CreateMap<AppointmentDto, AppointmentRowViewModel>()
            .ForMember(dest => dest.AppointmentType,   opt => opt.Ignore())
            .ForMember(dest => dest.HasMedicalRecord,  opt => opt.Ignore())
            .ForMember(dest => dest.HasInvoice,        opt => opt.Ignore());

        CreateMap<AuditInfoDto, AuditInfoViewModel>();
    }
}
