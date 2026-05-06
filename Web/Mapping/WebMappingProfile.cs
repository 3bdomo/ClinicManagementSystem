using AutoMapper;
using BLL.DTOs.Appointment;
using BLL.DTOs.Patient;
using BLL.DTOs.Shared;
using BLL.DTOs.User;
using Common.Enums;
using Web.ViewModel;
using Web.ViewModels.Appointment;

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


        CreateMap<AppointmentDto, AppointmentRowViewModel>();

        // ─────────────────────────────────────────────
        // Appointment Web mappings
        // ─────────────────────────────────────────────

        CreateMap<AppointmentDto, AppointmentDetailsViewModel>()
            .ForMember(dest => dest.CanEdit, opt => opt.Ignore())
            .ForMember(dest => dest.CanCancel, opt => opt.Ignore())
            .ForMember(dest => dest.CanStart, opt => opt.Ignore())
            .ForMember(dest => dest.CanComplete, opt => opt.Ignore())
            .ForMember(dest => dest.CanDelete, opt => opt.Ignore());

        CreateMap<AppointmentDto, EditAppointmentViewModel>()
            .ForMember(dest => dest.CurrentStatus,
                opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.ScheduleType,
                opt => opt.MapFrom(src => src.AppointmentType == AppointmentType.Surgery
                    ? ScheduleType.Surgery
                    : ScheduleType.Consultation))
            .ForMember(dest => dest.AvailableSlots, opt => opt.Ignore());

        CreateMap<EditAppointmentViewModel, UpdateAppointmentDto>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.AppointmentDate,
                opt => opt.MapFrom(src => src.AppointmentDate))
            .ForMember(dest => dest.DurationMinutes,
                opt => opt.MapFrom(src => src.DurationMinutes))
            .ForMember(dest => dest.Notes,
                opt => opt.MapFrom(src => src.Notes));

        CreateMap<BookConfirmViewModel, CreateAppointmentDto>()
            .ForMember(dest => dest.DoctorId,
                opt => opt.MapFrom(src => src.DoctorId))
            .ForMember(dest => dest.PatientId,
                opt => opt.MapFrom(src => src.PatientId))
            .ForMember(dest => dest.DoctorScheduleId,
                opt => opt.MapFrom(src => src.DoctorScheduleId))
            .ForMember(dest => dest.AppointmentDate,
                opt => opt.MapFrom(src => src.AppointmentDate))
            .ForMember(dest => dest.DurationMinutes,
                opt => opt.MapFrom(src => src.DurationMinutes))
            .ForMember(dest => dest.AppointmentType,
                opt => opt.MapFrom(src => src.AppointmentType))
            .ForMember(dest => dest.Notes,
                opt => opt.MapFrom(src => src.Notes));

        CreateMap<AppointmentDto, CancelAppointmentViewModel>()
            .ForMember(dest => dest.AppointmentId,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.DoctorName,
                opt => opt.MapFrom(src => src.DoctorName ?? string.Empty))
            .ForMember(dest => dest.PatientName,
                opt => opt.MapFrom(src => src.PatientName ?? string.Empty))
            .ForMember(dest => dest.AppointmentDate,
                opt => opt.MapFrom(src => src.AppointmentDate))
            .ForMember(dest => dest.CancellationReason,
                opt => opt.MapFrom(src => src.CancellationReason));

        CreateMap<AuditInfoDto, AuditInfoViewModel>();
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
