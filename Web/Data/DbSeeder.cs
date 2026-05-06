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

        
        

        
        string[] roles = { "Admin", "Doctor", "Receptionist", "Patient" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        
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
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        
        if (!context.Doctors.Any())
        {
            var doctorUser = new ApplicationUser
            {
                UserName = "doctor@clinic.com",
                Email = "doctor@clinic.com",
                FullName = "Dr. Ahmed Ali",
                UserRole = UserRole.Doctor,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };
            if ((await userManager.CreateAsync(doctorUser, "Doctor@123")).Succeeded)
            {
                await userManager.AddToRoleAsync(doctorUser, "Doctor");
                context.Doctors.Add(new Doctor
                {
                    ApplicationUserId = doctorUser.Id,
                    FullName = "Dr. Ahmed Ali",
                    Specialization = Specialization.General,
                    ConsultationFee = 500,
                    IsAvailable = true,
                    Phone = "01000000001"
                });
            }
        }

        
        if (!context.Receptionists.Any())
        {
            var recUser = new ApplicationUser
            {
                UserName = "reception@clinic.com",
                Email = "reception@clinic.com",
                FullName = "Mona Adel",
                UserRole = UserRole.Receptionist,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };
            if ((await userManager.CreateAsync(recUser, "Rec@123")).Succeeded)
            {
                await userManager.AddToRoleAsync(recUser, "Receptionist");
                context.Receptionists.Add(new Receptionist
                {
                    ApplicationUserId = recUser.Id,
                    FullName = "Mona Adel",
                    Phone = "01000000002",
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        
        if (!context.ProcedureTypes.Any())
        {
            context.ProcedureTypes.AddRange(
                new ProcedureType { Name = "Consultation", Description = "General Checkup", DefaultCost = 500 },
                new ProcedureType { Name = "X-Ray", Description = "Radiology", DefaultCost = 300 },
                new ProcedureType { Name = "Blood Test", Description = "Laboratory", DefaultCost = 150 }
            );
        }

        await context.SaveChangesAsync();

        
        if (!context.Patients.Any())
        {
            var patientUser = new ApplicationUser
            {
                UserName = "patient@clinic.com",
                Email = "patient@clinic.com",
                FullName = "Sara Tarek",
                UserRole = UserRole.Patient,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };
            if ((await userManager.CreateAsync(patientUser, "Patient@123")).Succeeded)
            {
                await userManager.AddToRoleAsync(patientUser, "Patient");
                context.Patients.Add(new Patient
                {
                    ApplicationUserId = patientUser.Id,
                    FullName = "Sara Tarek",
                    DateOfBirth = new DateOnly(1995, 5, 20),
                    Gender = Gender.Female,
                    NationalId = "29505201234567",
                    Phone = "01000000003",
                    Address = "Cairo, Egypt",
                    CreatedAt = DateTime.UtcNow
                });
            }
            await context.SaveChangesAsync(); 
        }

        
        var doctor = await context.Doctors.FirstOrDefaultAsync(d => d.FullName == "Dr. Ahmed Ali");
        var patient = await context.Patients.FirstOrDefaultAsync(p => p.FullName == "Sara Tarek");
        var procType = await context.ProcedureTypes.FirstOrDefaultAsync(p => p.Name == "Consultation");

        if (doctor != null && patient != null)
        {
            
            if (!context.DoctorSchedules.Any())
            {
                context.DoctorSchedules.Add(new DoctorSchedule
                {
                    DoctorId = doctor.Id,
                    ScheduleType = ScheduleType.Consultation,
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(17, 0),
                    SlotMinutes = 30,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
            }

            
            if (!context.Appointments.Any())
            {
                var schedule = await context.DoctorSchedules.FirstOrDefaultAsync();
                context.Appointments.Add(new Appointment
                {
                    PatientId = patient.Id,
                    DoctorId = doctor.Id,
                    DoctorScheduleId = schedule?.Id,
                    AppointmentDate = DateTime.Today.AddHours(10), 
                    DurationMinutes = 30,
                    AppointmentType = AppointmentType.Consultation,
                    Status = AppointmentStatus.Completed,
                    CreatedAt = DateTime.UtcNow
                });
                
                context.Appointments.Add(new Appointment
                {
                    PatientId = patient.Id,
                    DoctorId = doctor.Id,
                    DoctorScheduleId = schedule?.Id,
                    AppointmentDate = DateTime.Today.AddHours(14), 
                    DurationMinutes = 30,
                    AppointmentType = AppointmentType.Consultation,
                    Status = AppointmentStatus.Completed,
                    CreatedAt = DateTime.UtcNow
                });
                
                await context.SaveChangesAsync();
            }

            
            if (!context.MedicalRecords.Any())
            {
                var appointment = await context.Appointments.FirstOrDefaultAsync();
                if (appointment != null)
                {
                    var record = new MedicalRecord
                    {
                        PatientId = patient.Id,
                        DoctorId = doctor.Id,
                        AppointmentId = appointment.Id,
                        Diagnosis = "Common Cold",
                        Notes = "Patient needs to rest and take prescribed medication.",
                        VisitDate = appointment.AppointmentDate,
                        FollowUpDate = DateTime.Today.AddDays(7),
                        CreatedAt = DateTime.UtcNow
                    };
                    context.MedicalRecords.Add(record);
                    await context.SaveChangesAsync();

                    if (procType != null)
                    {
                        record.Procedures.Add(new Procedure
                        {
                            MedicalRecordId = record.Id,
                            ProcedureTypeId = procType.Id,
                            PerformedAt = appointment.AppointmentDate,
                            DurationMinutes = 30,
                            Cost = procType.DefaultCost,
                            CreatedAt = DateTime.UtcNow
                        });
                        await context.SaveChangesAsync();
                    }
                }
            }

            
            if (!context.Invoices.Any())
            {
                var appointment = await context.Appointments.FirstOrDefaultAsync();
                if (appointment != null)
                {
                    var invoice = new Invoice
                    {
                        PatientId = patient.Id,
                        AppointmentId = appointment.Id,
                        TotalAmount = doctor.ConsultationFee + (procType?.DefaultCost ?? 0),
                        Status = InvoiceStatus.Unpaid,
                        CreatedAt = DateTime.UtcNow
                    };
                    context.Invoices.Add(invoice);
                    
                    invoice.Items.Add(new InvoiceItem
                    {
                        Description = "Consultation Fee",
                        Quantity = 1,
                        UnitPrice = doctor.ConsultationFee,
                        ItemType = InvoiceItemType.Consultation
                    });

                    if (procType != null)
                    {
                        invoice.Items.Add(new InvoiceItem
                        {
                            Description = procType.Name,
                            Quantity = 1,
                            UnitPrice = procType.DefaultCost,
                            ItemType = InvoiceItemType.Procedure
                        });
                    }
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
