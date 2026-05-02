
using AutoMapper;
using BLL.DTOs.Patient;
using BLL.DTOs;
using ClinicSystem.DAL.Models;

namespace BLL.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Patient, PatientDTO>().ReverseMap();


        CreateMap<Doctor, DoctorDto>()
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName ?? s.ApplicationUser.FullName))
            .ForMember(d => d.Phone, o => o.MapFrom(s => s.ApplicationUser.PhoneNumber));

        CreateMap<DoctorSchedule, DoctorScheduleDto>()
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor.ApplicationUser.FullName))
            .ReverseMap();

        CreateMap<Appointment, AppointmentDto>()
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient.FullName))
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor.ApplicationUser.FullName));

        CreateMap<MedicalRecord, MedicalRecordDto>()
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient.FullName))
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor.ApplicationUser.FullName));

        CreateMap<RecordAttachment, AttachmentDto>().ReverseMap();

        
        CreateMap<ProcedureType, ProcedureTypeDto>().ReverseMap();

        CreateMap<Procedure, ProcedureDto>()
            .ForMember(d => d.ProcedureTypeName, o => o.MapFrom(s => s.ProcedureType.Name));

        CreateMap<Invoice, InvoiceDto>()
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient.FullName));

        CreateMap<InvoiceItem, InvoiceItemDto>()
            .ForMember(d => d.Total, o => o.MapFrom(s => s.Quantity * s.UnitPrice));

        CreateMap<ApplicationUser, UserDto>()
            .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.PhoneNumber));
    }
}