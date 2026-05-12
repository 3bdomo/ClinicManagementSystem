using AutoMapper;
using BLL.DTOs;
using BLL.DTOs.Appointment;
using BLL.DTOs.Auth;
using BLL.DTOs.Doctor;
using BLL.DTOs.MedicalRecord;
using BLL.DTOs.Patient;
using BLL.DTOs.Procedure;
using BLL.DTOs.Receptionist;
using BLL.DTOs.Shared;
using BLL.DTOs.User;
using BLL.DTOs.Billing;
using ClinicSystem.DAL.Models;
using Common.Enums;

namespace ClinicSystem.BLL.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateInvoiceDto, Invoice>();
        CreateMap<CreateInvoiceItemDto, InvoiceItem>();

        CreateMap<Patient, PatientDto>();

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
            .ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore())
            .ForMember(dest => dest.BloodType, opt => opt.MapFrom(src => src.BloodType));

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
            .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore())
            .ForMember(dest => dest.BloodType, opt => opt.MapFrom(src => src.BloodType));

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
            .ForMember(dest => dest.AuditInfo,
                       opt => opt.MapFrom(src => src));

        CreateMap<Patient, AuditInfoDto>();
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        

        
        
        
        
        
        
        CreateMap<ProcedureType, ProcedureTypeDto>();

        CreateMap<ProcedureTypeDto, ProcedureType>()
            .ForMember(dest => dest.Procedures, opt => opt.Ignore());

        CreateMap<CreateProcedureTypeDto, ProcedureType>()
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

        CreateMap<CreateProcedureDto, Procedure>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.MedicalRecord, opt => opt.Ignore())
            .ForMember(dest => dest.ProcedureType, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
        
        
        
        
        
        

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        

        CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.PhoneNumber,
                       opt => opt.MapFrom(src => src.PhoneNumber));

        CreateMap<CreateUserDto, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.UserRole))
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

        CreateMap<RegisterDto, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Doctor, opt => opt.Ignore())
            .ForMember(dest => dest.Patient, opt => opt.Ignore())
            .ForMember(dest => dest.Receptionist, opt => opt.Ignore());

        CreateMap<RegisterDto, Patient>()
            .ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore())
            .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Appointments, opt => opt.Ignore())
            .ForMember(dest => dest.MedicalRecords, opt => opt.Ignore())
            .ForMember(dest => dest.Invoices, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.BloodType, opt => opt.MapFrom(src => src.BloodType));

        CreateMap<PatientRegisterDto, Patient>()
            .ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore())
            .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Appointments, opt => opt.Ignore())
            .ForMember(dest => dest.MedicalRecords, opt => opt.Ignore())
            .ForMember(dest => dest.Invoices, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.BloodType, opt => opt.MapFrom(src => src.BloodType));



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
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

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

        CreateMap<Patient, AuditInfoDto>()
            .ForMember(dest => dest.CreatedByName, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedByName, opt => opt.Ignore());

        CreateMap<Invoice, InvoiceDto>()
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.FullName : string.Empty));

        CreateMap<InvoiceItem, InvoiceItemDto>();

        CreateMap<Procedure, AuditInfoDto>()
            .ForMember(dest => dest.CreatedByName, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedByName, opt => opt.Ignore());


        
        

        CreateMap<DoctorDto, Doctor>()
            .ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore())
            .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore())
            .ForMember(dest => dest.DoctorSchedules, opt => opt.Ignore())
            .ForMember(dest => dest.Appointments, opt => opt.Ignore())
            .ForMember(dest => dest.MedicalRecords, opt => opt.Ignore());

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
          


        CreateMap<DoctorScheduleDto, DoctorSchedule>()
            .ForMember(dest => dest.Doctor, opt => opt.Ignore())
            .ForMember(dest => dest.Appointments, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

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


        // Map VisitDate -> VisitedDate and counts for attachments/procedures
        CreateMap<MedicalRecord, MedicalRecordDto>()
            .ForMember(dest => dest.PatientName,
                opt => opt.MapFrom(src => src.Patient != null
                    ? src.Patient.FullName
                    : string.Empty))
            .ForMember(dest => dest.DoctorName,
                opt => opt.MapFrom(src => src.Doctor != null
                    ? src.Doctor.FullName
                    : string.Empty))
            .ForMember(dest => dest.VisitedDate,
                opt => opt.MapFrom(src => src.VisitDate))
            .ForMember(dest => dest.ProceduresCount,
                opt => opt.MapFrom(src => src.Procedures != null ? src.Procedures.Count : 0))
            .ForMember(dest => dest.AttachmentsCount,
                opt => opt.MapFrom(src => src.Attachments != null ? src.Attachments.Count : 0));

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

            // Mapping from CreateMedicalRecordDto -> MedicalRecord
        // Note: DTO uses "VisitedDate" while entity uses "VisitDate" so map explicitly.
        CreateMap<CreateMedicalRecordDto, MedicalRecord>()
            .ForMember(dest => dest.VisitDate, opt => opt.MapFrom(src => src.VisitedDate))
            .ForMember(dest => dest.Patient, opt => opt.Ignore())
            .ForMember(dest => dest.Doctor, opt => opt.Ignore())
            .ForMember(dest => dest.Appointment, opt => opt.Ignore())
            .ForMember(dest => dest.Attachments, opt => opt.Ignore())
            .ForMember(dest => dest.Procedures, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

        


        CreateMap<Receptionist, ReceptionistDto>()
            .ForMember(dest => dest.PhoneNumber,
                       opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.Email,
                       opt => opt.MapFrom(src => src.ApplicationUser != null
                                                 ? src.ApplicationUser.Email
                                                 : null));




        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.DoctorName,
                opt => opt.MapFrom(src => src.Doctor != null
                    ? src.Doctor.FullName
                    : null))
            .ForMember(dest => dest.PatientName,
                opt => opt.MapFrom(src => src.Patient != null
                    ? src.Patient.FullName
                    : null))
            .ForMember(dest => dest.DoctorSpecialization,
                opt => opt.MapFrom(src => src.Doctor != null
                    ? src.Doctor.Specialization
                    : (Specialization?)null))
            .ForMember(dest => dest.HasMedicalRecord,
                opt => opt.MapFrom(src => src.MedicalRecord != null))
            .ForMember(dest => dest.HasInvoice,
                opt => opt.MapFrom(src => src.Invoice != null))
            .ForMember(dest => dest.MedicalRecordId,
                opt => opt.MapFrom(src => src.MedicalRecord != null
                    ? src.MedicalRecord.Id
                    : (int?)null))
            .ForMember(dest => dest.InvoiceId,
                opt => opt.MapFrom(src => src.Invoice != null
                    ? src.Invoice.Id
                    : (int?)null));

        CreateMap<Appointment, AppointmentHistoryDto>()
            .ForMember(dest => dest.DoctorName,
                opt => opt.MapFrom(src => src.Doctor != null
                    ? src.Doctor.FullName
                    : null))
            .ForMember(dest => dest.DoctorSpecialization,
                opt => opt.MapFrom(src => src.Doctor != null
                    ? src.Doctor.Specialization
                    : (Specialization?)null))
            .ForMember(dest => dest.HasMedicalRecord,
                opt => opt.MapFrom(src => src.MedicalRecord != null))
            .ForMember(dest => dest.HasInvoice,
                opt => opt.MapFrom(src => src.Invoice != null));

        CreateMap<CreateAppointmentDto, Appointment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => AppointmentStatus.Waiting))
            .ForMember(dest => dest.CancellationReason, opt => opt.Ignore())
            .ForMember(dest => dest.Patient, opt => opt.Ignore())
            .ForMember(dest => dest.Doctor, opt => opt.Ignore())
            .ForMember(dest => dest.DoctorSchedule, opt => opt.Ignore())
            .ForMember(dest => dest.MedicalRecord, opt => opt.Ignore())
            .ForMember(dest => dest.Invoice, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

       
        
        
        
        CreateMap<AppointmentDto, Appointment>()
            .ForMember(dest => dest.Doctor, opt => opt.Ignore())
            .ForMember(dest => dest.Patient, opt => opt.Ignore())
            .ForMember(dest => dest.DoctorSchedule, opt => opt.Ignore())
            .ForMember(dest => dest.MedicalRecord, opt => opt.Ignore())
            .ForMember(dest => dest.Invoice, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());


    }
}