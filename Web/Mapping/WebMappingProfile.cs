using AutoMapper;
using BLL.DTOs.Appointment;
using BLL.DTOs.Patient;
using BLL.DTOs.Shared;
using BLL.DTOs.User;
using Common.Enums;
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


        CreateMap<PatientFormViewModel, PatientDto>()
            .ForMember(dest => dest.BloodType, opt => opt.MapFrom(src => FormatBloodType(src.BloodType)));
        CreateMap<PatientDto, PatientFormViewModel>()
            .ForMember(dest => dest.BloodType, opt => opt.MapFrom(src => ParseBloodType(src.BloodType)));

       
        CreateMap<PatientHistoryDto, PatientDetailsViewModel>()
            .ForMember(dest => dest.Patient,        opt => opt.MapFrom(src => src.Patient))
            .ForMember(dest => dest.Appointments,   opt => opt.MapFrom(src => src.Appointments))
            .ForMember(dest => dest.MedicalRecords, opt => opt.Ignore())
            .ForMember(dest => dest.Invoices,       opt => opt.Ignore())
            .ForMember(dest => dest.AuditInfo,      opt => opt.Ignore());

        CreateMap<PatientDto, PatientDetailsViewModel>()
            .ForMember(dest => dest.Patient,        opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Appointments,   opt => opt.Ignore())
            .ForMember(dest => dest.MedicalRecords, opt => opt.Ignore())
            .ForMember(dest => dest.Invoices,       opt => opt.Ignore())
            .ForMember(dest => dest.AuditInfo,      opt => opt.Ignore());


        CreateMap<AppointmentDto, AppointmentRowViewModel>()
            .ForMember(dest => dest.AppointmentType,   opt => opt.Ignore())
            .ForMember(dest => dest.HasMedicalRecord,  opt => opt.Ignore())
            .ForMember(dest => dest.HasInvoice,        opt => opt.Ignore());

        CreateMap<AuditInfoDto, AuditInfoViewModel>();

        CreateMap<BLL.DTOs.Procedure.ProcedureTypeDto, ProcedureTypeFormViewModel>();
        CreateMap<ProcedureTypeFormViewModel, BLL.DTOs.Procedure.ProcedureTypeDto>();
        CreateMap<ProcedureTypeFormViewModel, BLL.DTOs.Procedure.CreateProcedureTypeDto>();
        CreateMap<ProcedureTypeFormViewModel, BLL.DTOs.Procedure.UpdateProcedureTypeDto>();
    }

    private BloodType? ParseBloodType(string? val)
    {
        if (string.IsNullOrEmpty(val)) return null;
        if (val == "A+") return BloodType.A_Positive;
        if (val == "A-") return BloodType.A_Negative;
        if (val == "B+") return BloodType.B_Positive;
        if (val == "B-") return BloodType.B_Negative;
        if (val == "AB+") return BloodType.AB_Positive;
        if (val == "AB-") return BloodType.AB_Negative;
        if (val == "O+") return BloodType.O_Positive;
        if (val == "O-") return BloodType.O_Negative;
        if (Enum.TryParse<BloodType>(val, out var result)) return result;
        return null;
    }

    private string? FormatBloodType(BloodType? val)
    {
        if (!val.HasValue) return null;
        return val.Value switch
        {
            BloodType.A_Positive => "A+",
            BloodType.A_Negative => "A-",
            BloodType.B_Positive => "B+",
            BloodType.B_Negative => "B-",
            BloodType.AB_Positive => "AB+",
            BloodType.AB_Negative => "AB-",
            BloodType.O_Positive => "O+",
            BloodType.O_Negative => "O-",
            _ => val.Value.ToString()
        };
    }
}
