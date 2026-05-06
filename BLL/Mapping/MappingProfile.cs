using AutoMapper;
using BLL.DTOs;
using BLL.DTOs.Appointment;
using BLL.DTOs.Auth;
using BLL.DTOs.Patient;
using BLL.DTOs.Receptionist;
using BLL.DTOs.Shared;
using BLL.DTOs.User;
using ClinicSystem.DAL.Models;
using Common.Enums;

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
                       opt => opt.MapFrom(src => src.Appointments));

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

        // New Mappings for RegisterDto
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
            .ForMember(dest => dest.Id, opt => opt.Ignore());

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
            .ForMember(dest => dest.Id, opt => opt.Ignore());



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

        /*
         * Kept for compatibility if any old code still maps AppointmentDto to Appointment.
         * Main Create flow should use CreateAppointmentDto.
         */
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