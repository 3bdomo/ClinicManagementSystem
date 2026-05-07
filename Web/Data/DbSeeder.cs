using ClinicSystem.DAL.Models;
using Common.Enums;
using DAL.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ClinicDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        // 1. Roles
        string[] roles = { "Admin", "Doctor", "Receptionist", "Patient" };
        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

        // 2. Admin User
        var adminEmail = "admin@clinic.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Admin",
                UserRole = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded) await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // 3. Doctors (at least 2)
        if (await context.Doctors.CountAsync() < 2)
        {
            // First doctor
            var firstDoctorEmail = "doctor@clinic.com";
            var firstDoctorUser = await userManager.FindByEmailAsync(firstDoctorEmail);
            if (firstDoctorUser == null)
            {
                firstDoctorUser = new ApplicationUser
                {
                    UserName = firstDoctorEmail,
                    Email = firstDoctorEmail,
                    FullName = "Dr. Ahmed Ali",
                    UserRole = UserRole.Doctor,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(firstDoctorUser, "Doctor@123");
                await userManager.AddToRoleAsync(firstDoctorUser, "Doctor");
                var newDoctor = new Doctor
                {
                    ApplicationUserId = firstDoctorUser.Id,
                    FullName = "Dr. Ahmed Ali",
                    Specialization = Specialization.General,
                    ConsultationFee = 500,
                    IsAvailable = true,
                    Phone = "01000000001"
                };
                context.Doctors.Add(newDoctor);
                await context.SaveChangesAsync();
            }

            // Second doctor
            var secondDoctorEmail = "doctor2@clinic.com";
            var secondDoctorUser = await userManager.FindByEmailAsync(secondDoctorEmail);
            if (secondDoctorUser == null)
            {
                secondDoctorUser = new ApplicationUser
                {
                    UserName = secondDoctorEmail,
                    Email = secondDoctorEmail,
                    FullName = "Dr. Mona Hassan",
                    UserRole = UserRole.Doctor,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(secondDoctorUser, "Doctor@123");
                await userManager.AddToRoleAsync(secondDoctorUser, "Doctor");
                var anotherDoctor = new Doctor
                {
                    ApplicationUserId = secondDoctorUser.Id,
                    FullName = "Dr. Mona Hassan",
                    Specialization = Specialization.Pediatrics,
                    ConsultationFee = 450,
                    IsAvailable = true,
                    Phone = "01000000011"
                };
                context.Doctors.Add(anotherDoctor);
                await context.SaveChangesAsync();
            }
        }

        // 4. Receptionists (at least 2)
        if (await context.Receptionists.CountAsync() < 2)
        {
            var rec1Email = "reception@clinic.com";
            if (await userManager.FindByEmailAsync(rec1Email) == null)
            {
                var recUser1 = new ApplicationUser
                {
                    UserName = rec1Email,
                    Email = rec1Email,
                    FullName = "Mona Adel",
                    UserRole = UserRole.Receptionist,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(recUser1, "Rec@123");
                await userManager.AddToRoleAsync(recUser1, "Receptionist");
                context.Receptionists.Add(new Receptionist
                {
                    ApplicationUserId = recUser1.Id,
                    FullName = "Mona Adel",
                    Phone = "01000000002",
                    CreatedAt = DateTime.UtcNow
                });
            }

            var rec2Email = "reception2@clinic.com";
            if (await userManager.FindByEmailAsync(rec2Email) == null)
            {
                var recUser2 = new ApplicationUser
                {
                    UserName = rec2Email,
                    Email = rec2Email,
                    FullName = "Nader Samir",
                    UserRole = UserRole.Receptionist,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(recUser2, "Rec@123");
                await userManager.AddToRoleAsync(recUser2, "Receptionist");
                context.Receptionists.Add(new Receptionist
                {
                    ApplicationUserId = recUser2.Id,
                    FullName = "Nader Samir",
                    Phone = "01000000012",
                    CreatedAt = DateTime.UtcNow
                });
            }
            await context.SaveChangesAsync();
        }

        // 5. Procedure Types
        if (!context.ProcedureTypes.Any())
        {
            context.ProcedureTypes.AddRange(
                new ProcedureType { Name = "Consultation", Description = "General Checkup", DefaultCost = 500 },
                new ProcedureType { Name = "X-Ray", Description = "Radiology", DefaultCost = 300 },
                new ProcedureType { Name = "Blood Test", Description = "Laboratory", DefaultCost = 150 }
            );
            await context.SaveChangesAsync();
        }

        // 6. Patients (at least 2)
        if (await context.Patients.CountAsync() < 2)
        {
            // First patient
            var patient1Email = "patient@clinic.com";
            if (await userManager.FindByEmailAsync(patient1Email) == null)
            {
                var patUser1 = new ApplicationUser
                {
                    UserName = patient1Email,
                    Email = patient1Email,
                    FullName = "Sara Tarek",
                    UserRole = UserRole.Patient,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(patUser1, "Patient@123");
                await userManager.AddToRoleAsync(patUser1, "Patient");
                context.Patients.Add(new Patient
                {
                    ApplicationUserId = patUser1.Id,
                    FullName = "Sara Tarek",
                    DateOfBirth = new DateOnly(1995, 5, 20),
                    Gender = Gender.Female,
                    NationalId = "29505201234567",
                    Phone = "01000000003",
                    Address = "Cairo, Egypt",
                    CreatedAt = DateTime.UtcNow
                });
            }

            // Second patient
            var patient2Email = "patient2@clinic.com";
            if (await userManager.FindByEmailAsync(patient2Email) == null)
            {
                var patUser2 = new ApplicationUser
                {
                    UserName = patient2Email,
                    Email = patient2Email,
                    FullName = "Ahmed Mahmoud",
                    UserRole = UserRole.Patient,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(patUser2, "Patient@123");
                await userManager.AddToRoleAsync(patUser2, "Patient");
                context.Patients.Add(new Patient
                {
                    ApplicationUserId = patUser2.Id,
                    FullName = "Ahmed Mahmoud",
                    DateOfBirth = new DateOnly(1988, 11, 15),
                    Gender = Gender.Male,
                    NationalId = "28811151234567",
                    Phone = "01000000013",
                    Address = "Giza, Egypt",
                    CreatedAt = DateTime.UtcNow
                });
            }
            await context.SaveChangesAsync();
        }

        // Fetch needed objects after ensuring they exist
        var doctors = await context.Doctors.ToListAsync();
        var firstDoctor = doctors.FirstOrDefault(d => d.FullName.Contains("Ahmed Ali"));
        var secondDoctor = doctors.FirstOrDefault(d => d.FullName.Contains("Mona Hassan"));
        var patients = await context.Patients.ToListAsync();
        var firstPatient = patients.FirstOrDefault(p => p.FullName == "Sara Tarek");
        var secondPatient = patients.FirstOrDefault(p => p.FullName == "Ahmed Mahmoud");
        var consultationProc = await context.ProcedureTypes.FirstOrDefaultAsync(p => p.Name == "Consultation");
        var xrayProc = await context.ProcedureTypes.FirstOrDefaultAsync(p => p.Name == "X-Ray");

        // 7. Doctor Schedules (at least 2 for first doctor, plus maybe for second)
        if (await context.DoctorSchedules.CountAsync() < 2 && firstDoctor != null)
        {
            // Monday schedule
            if (!await context.DoctorSchedules.AnyAsync(ds => ds.DoctorId == firstDoctor.Id && ds.DayOfWeek == DayOfWeek.Monday))
            {
                context.DoctorSchedules.Add(new DoctorSchedule
                {
                    DoctorId = firstDoctor.Id,
                    ScheduleType = ScheduleType.Consultation,
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(17, 0),
                    SlotMinutes = 30,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            // Wednesday schedule (second schedule for same doctor)
            if (!await context.DoctorSchedules.AnyAsync(ds => ds.DoctorId == firstDoctor.Id && ds.DayOfWeek == DayOfWeek.Wednesday))
            {
                context.DoctorSchedules.Add(new DoctorSchedule
                {
                    DoctorId = firstDoctor.Id,
                    ScheduleType = ScheduleType.Consultation,
                    DayOfWeek = DayOfWeek.Wednesday,
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(17, 0),
                    SlotMinutes = 30,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            // Optional: schedule for second doctor
            if (secondDoctor != null && !await context.DoctorSchedules.AnyAsync(ds => ds.DoctorId == secondDoctor.Id))
            {
                context.DoctorSchedules.Add(new DoctorSchedule
                {
                    DoctorId = secondDoctor.Id,
                    ScheduleType = ScheduleType.Consultation,
                    DayOfWeek = DayOfWeek.Tuesday,
                    StartTime = new TimeOnly(10, 0),
                    EndTime = new TimeOnly(16, 0),
                    SlotMinutes = 30,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }
            await context.SaveChangesAsync();
        }

        // Get schedules for first doctor
        var firstDoctorSchedules = await context.DoctorSchedules
            .Where(ds => ds.DoctorId == firstDoctor.Id)
            .ToListAsync();
        var mondaySchedule = firstDoctorSchedules.FirstOrDefault(ds => ds.DayOfWeek == DayOfWeek.Monday);
        var wednesdaySchedule = firstDoctorSchedules.FirstOrDefault(ds => ds.DayOfWeek == DayOfWeek.Wednesday);

        // 8. Appointments (at least 2)
        if (await context.Appointments.CountAsync() < 2 && firstDoctor != null && firstPatient != null)
        {
            // First appointment
            if (!await context.Appointments.AnyAsync())
            {
                var appt1 = new Appointment
                {
                    PatientId = firstPatient.Id,
                    DoctorId = firstDoctor.Id,
                    DoctorScheduleId = mondaySchedule?.Id,
                    AppointmentDate = DateTime.Today.AddHours(10),
                    DurationMinutes = 30,
                    AppointmentType = AppointmentType.Consultation,
                    Status = AppointmentStatus.Completed,
                    CreatedAt = DateTime.UtcNow
                };
                context.Appointments.Add(appt1);
                await context.SaveChangesAsync();
            }

            // Second appointment (with second patient, using Wednesday schedule)
            if (secondPatient != null && wednesdaySchedule != null)
            {
                var existingSecond = await context.Appointments
                    .FirstOrDefaultAsync(a => a.PatientId == secondPatient.Id && a.DoctorId == firstDoctor.Id);
                if (existingSecond == null)
                {
                    var appt2 = new Appointment
                    {
                        PatientId = secondPatient.Id,
                        DoctorId = firstDoctor.Id,
                        DoctorScheduleId = wednesdaySchedule.Id,
                        AppointmentDate = DateTime.Today.AddDays(2).AddHours(11),
                        DurationMinutes = 30,
                        AppointmentType = AppointmentType.Consultation,
                        Status = AppointmentStatus.Waiting,
                        CreatedAt = DateTime.UtcNow
                    };
                    context.Appointments.Add(appt2);
                    await context.SaveChangesAsync();
                }
            }
        }

        // Fetch all appointments after seeding
        var appointments = await context.Appointments.ToListAsync();
        var firstAppointment = appointments.FirstOrDefault(a => a.PatientId == firstPatient?.Id);
        var secondAppointment = appointments.FirstOrDefault(a => a.PatientId == secondPatient?.Id);

        // 9. Medical Records (at least 2)
        if (await context.MedicalRecords.CountAsync() < 2)
        {
            // First medical record
            if (firstAppointment != null && !await context.MedicalRecords.AnyAsync(m => m.AppointmentId == firstAppointment.Id))
            {
                var record1 = new MedicalRecord
                {
                    PatientId = firstPatient.Id,
                    DoctorId = firstDoctor.Id,
                    AppointmentId = firstAppointment.Id,
                    Diagnosis = "Common Cold",
                    Notes = "Rest and fluids",
                    VisitDate = firstAppointment.AppointmentDate,
                    FollowUpDate = DateTime.Today.AddDays(7),
                    CreatedAt = DateTime.UtcNow
                };
                context.MedicalRecords.Add(record1);
                await context.SaveChangesAsync();

                if (consultationProc != null && !context.Procedures.Any(p => p.MedicalRecordId == record1.Id))
                {
                    context.Procedures.Add(new Procedure
                    {
                        MedicalRecordId = record1.Id,
                        ProcedureTypeId = consultationProc.Id,
                        PerformedAt = firstAppointment.AppointmentDate,
                        DurationMinutes = 30,
                        Cost = consultationProc.DefaultCost,
                        CreatedAt = DateTime.UtcNow
                    });
                    await context.SaveChangesAsync();
                }
            }

            // Second medical record
            if (secondAppointment != null && !await context.MedicalRecords.AnyAsync(m => m.AppointmentId == secondAppointment.Id))
            {
                var record2 = new MedicalRecord
                {
                    PatientId = secondPatient.Id,
                    DoctorId = firstDoctor.Id,
                    AppointmentId = secondAppointment.Id,
                    Diagnosis = "Fever and Cough",
                    Notes = "Prescribed antibiotics",
                    VisitDate = secondAppointment.AppointmentDate,
                    FollowUpDate = secondAppointment.AppointmentDate.AddDays(5),
                    CreatedAt = DateTime.UtcNow
                };
                context.MedicalRecords.Add(record2);
                await context.SaveChangesAsync();

                if (xrayProc != null && !context.Procedures.Any(p => p.MedicalRecordId == record2.Id))
                {
                    context.Procedures.Add(new Procedure
                    {
                        MedicalRecordId = record2.Id,
                        ProcedureTypeId = xrayProc.Id,
                        PerformedAt = secondAppointment.AppointmentDate,
                        DurationMinutes = 20,
                        Cost = xrayProc.DefaultCost,
                        CreatedAt = DateTime.UtcNow
                    });
                    await context.SaveChangesAsync();
                }
            }
        }

        // 10. Invoices (at least 2)
        if (await context.Invoices.CountAsync() < 2)
        {
            // Invoice for first appointment
            if (firstAppointment != null && !await context.Invoices.AnyAsync(i => i.AppointmentId == firstAppointment.Id))
            {
                var invoice1 = new Invoice
                {
                    PatientId = firstPatient.Id,
                    AppointmentId = firstAppointment.Id,
                    TotalAmount = firstDoctor.ConsultationFee + (consultationProc?.DefaultCost ?? 0),
                    Status = InvoiceStatus.Unpaid,
                    CreatedAt = DateTime.UtcNow
                };
                context.Invoices.Add(invoice1);
                await context.SaveChangesAsync();

                invoice1.Items.Add(new InvoiceItem
                {
                    Description = "Consultation Fee",
                    Quantity = 1,
                    UnitPrice = firstDoctor.ConsultationFee,
                    ItemType = InvoiceItemType.Consultation
                });
                if (consultationProc != null)
                {
                    invoice1.Items.Add(new InvoiceItem
                    {
                        Description = consultationProc.Name,
                        Quantity = 1,
                        UnitPrice = consultationProc.DefaultCost,
                        ItemType = InvoiceItemType.Procedure
                    });
                }
                await context.SaveChangesAsync();
            }

            // Invoice for second appointment
            if (secondAppointment != null && !await context.Invoices.AnyAsync(i => i.AppointmentId == secondAppointment.Id))
            {
                var invoice2 = new Invoice
                {
                    PatientId = secondPatient.Id,
                    AppointmentId = secondAppointment.Id,
                    TotalAmount = firstDoctor.ConsultationFee + (xrayProc?.DefaultCost ?? 0),
                    Status = InvoiceStatus.Paid,
                    PaidAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };
                context.Invoices.Add(invoice2);
                await context.SaveChangesAsync();

                invoice2.Items.Add(new InvoiceItem
                {
                    Description = "Consultation Fee",
                    Quantity = 1,
                    UnitPrice = firstDoctor.ConsultationFee,
                    ItemType = InvoiceItemType.Consultation
                });
                if (xrayProc != null)
                {
                    invoice2.Items.Add(new InvoiceItem
                    {
                        Description = xrayProc.Name,
                        Quantity = 1,
                        UnitPrice = xrayProc.DefaultCost,
                        ItemType = InvoiceItemType.Procedure
                    });
                }
                await context.SaveChangesAsync();
            }
        }
    }
}