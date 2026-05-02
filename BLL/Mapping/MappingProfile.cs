using AutoMapper;
using BLL.DTOs;
using BLL.DTOs.Patient;
using ClinicSystem.DAL.Models;

namespace ClinicSystem.BLL.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {

        CreateMap<Patient, PatientDto>();
          //  .ForMember(dest => dest.ApplicationUserId,opt => opt.MapFrom(src => src.ApplicationUserId));

        CreateMap<PatientDto, Patient>()
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Appointments, opt => opt.Ignore())
            .ForMember(dest => dest.MedicalRecords, opt => opt.Ignore())
            .ForMember(dest => dest.Invoices, opt => opt.Ignore())
            .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore())
            .ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore());

        CreateMap<PatientRegisterDto, Patient>()
            .ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Appointments, opt => opt.Ignore())
            .ForMember(dest => dest.MedicalRecords, opt => opt.Ignore())
            .ForMember(dest => dest.Invoices, opt => opt.Ignore())
            .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore());

        CreateMap<PatientRegisterDto, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Doctor, opt => opt.Ignore())
            .ForMember(dest => dest.Patient, opt => opt.Ignore())
            .ForMember(dest => dest.Receptionist, opt => opt.Ignore());

        CreateMap<Patient, PatientHistoryDto>()
            .ForMember(dest => dest.Patient,
                       opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Appointments,
                       opt => opt.MapFrom(src => src.Appointments))
            .ForMember(dest => dest.MedicalRecords,
                       opt => opt.MapFrom(src => src.MedicalRecords))
            .ForMember(dest => dest.Invoices,
                       opt => opt.MapFrom(src => src.Invoices));

        CreateMap<Doctor, DoctorDto>();

        CreateMap<DoctorDto, Doctor>()
            .ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore())
            .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore())
            .ForMember(dest => dest.DoctorSchedules, opt => opt.Ignore())
            .ForMember(dest => dest.Appointments, opt => opt.Ignore())
            .ForMember(dest => dest.MedicalRecords, opt => opt.Ignore());

        CreateMap<Doctor, UserDto>()
            .ForMember(dest => dest.Id,
                       opt => opt.MapFrom(src => src.ApplicationUserId))
            .ForMember(dest => dest.FullName,
                       opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.IsActive,
                       opt => opt.MapFrom(src => src.IsAvailable))
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
            .ForMember(dest => dest.UserRole, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        CreateMap<DoctorSchedule, DoctorScheduleDto>()
            .ForMember(dest => dest.DoctorName,
                       opt => opt.MapFrom(src => src.Doctor != null
                                                 ? src.Doctor.FullName
                                                 : string.Empty));

        CreateMap<DoctorScheduleDto, DoctorSchedule>()
            .ForMember(dest => dest.Doctor, opt => opt.Ignore())
            .ForMember(dest => dest.Appointments, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.PatientName,
                       opt => opt.MapFrom(src => src.Patient != null
                                                 ? src.Patient.FullName
                                                 : string.Empty))
            .ForMember(dest => dest.DoctorName,
                       opt => opt.MapFrom(src => src.Doctor != null
                                                 ? src.Doctor.FullName
                                                 : string.Empty));

        CreateMap<AppointmentDto, Appointment>()
            .ForMember(dest => dest.Patient, opt => opt.Ignore())
            .ForMember(dest => dest.Doctor, opt => opt.Ignore())
            .ForMember(dest => dest.DoctorSchedule, opt => opt.Ignore())
            .ForMember(dest => dest.MedicalRecord, opt => opt.Ignore())
            .ForMember(dest => dest.Invoice, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

        CreateMap<MedicalRecord, MedicalRecordDto>()
            .ForMember(dest => dest.PatientName,
                       opt => opt.MapFrom(src => src.Patient != null
                                                 ? src.Patient.FullName
                                                 : string.Empty))
            .ForMember(dest => dest.DoctorName,
                       opt => opt.MapFrom(src => src.Doctor != null
                                                 ? src.Doctor.FullName
                                                 : string.Empty));

        CreateMap<MedicalRecordDto, MedicalRecord>()
            .ForMember(dest => dest.Patient, opt => opt.Ignore())
            .ForMember(dest => dest.Doctor, opt => opt.Ignore())
            .ForMember(dest => dest.Appointment, opt => opt.Ignore())
            .ForMember(dest => dest.Attachments, opt => opt.Ignore())
            .ForMember(dest => dest.Procedures, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

        CreateMap<RecordAttachment, AttachmentDto>();

        CreateMap<AttachmentDto, RecordAttachment>()
            .ForMember(dest => dest.MedicalRecord, opt => opt.Ignore())
            .ForMember(dest => dest.UploadedAt, opt => opt.Ignore());

        CreateMap<ProcedureType, ProcedureTypeDto>();

        CreateMap<ProcedureTypeDto, ProcedureType>()
            .ForMember(dest => dest.Procedures, opt => opt.Ignore());

        CreateMap<Procedure, ProcedureDto>()
            .ForMember(dest => dest.ProcedureTypeName,
                       opt => opt.MapFrom(src => src.ProcedureType != null
                                                 ? src.ProcedureType.Name
                                                 : string.Empty));

        CreateMap<ProcedureDto, Procedure>()
            .ForMember(dest => dest.MedicalRecord, opt => opt.Ignore())
            .ForMember(dest => dest.ProcedureType, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

        CreateMap<Invoice, InvoiceDto>()
            .ForMember(dest => dest.PatientName,
                       opt => opt.MapFrom(src => src.Patient != null
                                                 ? src.Patient.FullName
                                                 : string.Empty));

        CreateMap<InvoiceDto, Invoice>()
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.Patient, opt => opt.Ignore())
            .ForMember(dest => dest.Appointment, opt => opt.Ignore())
            .ForMember(dest => dest.Items, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

        CreateMap<InvoiceItem, InvoiceItemDto>()
            .ForMember(dest => dest.Total,
                       opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));

        CreateMap<InvoiceItemDto, InvoiceItem>()
            .ForMember(dest => dest.Invoice, opt => opt.Ignore());

        CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.PhoneNumber,
                       opt => opt.MapFrom(src => src.PhoneNumber));

        CreateMap<CreateUserDto, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Doctor, opt => opt.Ignore())
            .ForMember(dest => dest.Patient, opt => opt.Ignore())
            .ForMember(dest => dest.Receptionist, opt => opt.Ignore());

        CreateMap<CreateUserDto, Doctor>()
            .ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore())
            .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore())
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.DoctorSchedules, opt => opt.Ignore())
            .ForMember(dest => dest.Appointments, opt => opt.Ignore())
            .ForMember(dest => dest.MedicalRecords, opt => opt.Ignore());

        CreateMap<CreateUserDto, Receptionist>()
            .ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore())
            .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

        CreateMap<UpdateUserDto, ApplicationUser>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.UserName, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.UserRole, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Doctor, opt => opt.Ignore())
            .ForMember(dest => dest.Patient, opt => opt.Ignore())
            .ForMember(dest => dest.Receptionist, opt => opt.Ignore());

        CreateMap<Receptionist, UserDto>()
            .ForMember(dest => dest.Id,
                       opt => opt.MapFrom(src => src.ApplicationUserId))
            .ForMember(dest => dest.FullName,
                       opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.IsActive,
                       opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
            .ForMember(dest => dest.UserRole, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        CreateMap<MedicalRecord, AuditInfoDto>()
            .ForMember(dest => dest.CreatedByName, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedByName, opt => opt.Ignore());

        CreateMap<Appointment, AuditInfoDto>()
            .ForMember(dest => dest.CreatedByName, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedByName, opt => opt.Ignore());

        CreateMap<DoctorSchedule, AuditInfoDto>()
            .ForMember(dest => dest.CreatedByName, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedByName, opt => opt.Ignore());

        CreateMap<Invoice, AuditInfoDto>()
            .ForMember(dest => dest.CreatedByName, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedByName, opt => opt.Ignore());

        CreateMap<Procedure, AuditInfoDto>()
            .ForMember(dest => dest.CreatedByName, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedByName, opt => opt.Ignore());
    }
}