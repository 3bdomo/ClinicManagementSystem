using ClinicSystem.DAL.Models;
using Common.Enums;
using DAL.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Web.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context     = serviceProvider.GetRequiredService<ClinicDbContext>();
        
        // Ensure database is created and migrations are applied
        await context.Database.MigrateAsync();

        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = { "Admin", "Doctor", "Receptionist", "Patient" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var adminEmail = "admin@clinic.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                UserName       = adminEmail,
                Email          = adminEmail,
                FullName       = "System Admin",
                UserRole       = UserRole.Admin,
                IsActive       = true,
                CreatedAt      = DateTime.UtcNow,
                EmailConfirmed = true
            };
            var res = await userManager.CreateAsync(admin, "Admin@123");
            if (res.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        if (!context.Receptionists.Any())
        {
            var receptionistsData = new[]
            {
                (Email: "reception1@clinic.com", Name: "Mona Adel",   Phone: "01000000010"),
                (Email: "reception2@clinic.com", Name: "Dina Kamal",  Phone: "01000000011"),
                (Email: "reception3@clinic.com", Name: "Heba Nasser", Phone: "01000000012"),
                (Email: "reception4@clinic.com", Name: "Reem Samir",  Phone: "01000000013"),
            };

            foreach (var r in receptionistsData)
            {
                var recUser = new ApplicationUser
                {
                    UserName       = r.Email,
                    Email          = r.Email,
                    FullName       = r.Name,
                    UserRole       = UserRole.Receptionist,
                    IsActive       = true,
                    CreatedAt      = DateTime.UtcNow,
                    EmailConfirmed = true
                };
                var res = await userManager.CreateAsync(recUser, "Receptionist@123");
                if (res.Succeeded)
                {
                    await userManager.AddToRoleAsync(recUser, "Receptionist");
                    context.Receptionists.Add(new Receptionist
                    {
                        ApplicationUserId = recUser.Id,
                        FullName          = r.Name,
                        Phone             = r.Phone,
                        IsActive          = true,
                        CreatedAt         = DateTime.UtcNow
                    });
                }
            }
            await context.SaveChangesAsync();
        }

        if (!context.ProcedureTypes.Any())
        {
            context.ProcedureTypes.AddRange(
                new ProcedureType { Name = "Consultation",   Description = "General Consultation",      DefaultCost = 500,  IsActive = true },
                new ProcedureType { Name = "X-Ray",          Description = "Radiology X-Ray",           DefaultCost = 300,  IsActive = true },
                new ProcedureType { Name = "Blood Test",     Description = "Complete Blood Count",      DefaultCost = 150,  IsActive = true },
                new ProcedureType { Name = "ECG",            Description = "Electrocardiogram",         DefaultCost = 200,  IsActive = true },
                new ProcedureType { Name = "Ultrasound",     Description = "Abdominal Ultrasound",      DefaultCost = 450,  IsActive = true },
                new ProcedureType { Name = "Dressing",       Description = "Wound Dressing",            DefaultCost = 100,  IsActive = true }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Doctors.Any())
        {
            var doctorsData = new[]
            {
                (Email: "dr.ahmed@clinic.com",   Name: "Dr. Ahmed Ali",      Spec: Specialization.General,       Fee: 500m,  Phone: "01100000001"),
                (Email: "dr.sara@clinic.com",    Name: "Dr. Sara Mohamed",   Spec: Specialization.Pediatrics,    Fee: 700m,  Phone: "01100000002"),
                (Email: "dr.khaled@clinic.com",  Name: "Dr. Khaled Hassan",  Spec: Specialization.Orthopedics,   Fee: 600m,  Phone: "01100000003"),
                (Email: "dr.nour@clinic.com",    Name: "Dr. Nour Ibrahim",   Spec: Specialization.Dermatology,   Fee: 550m,  Phone: "01100000004"),
            };

            foreach (var d in doctorsData)
            {
                var user = new ApplicationUser
                {
                    UserName       = d.Email,
                    Email          = d.Email,
                    FullName       = d.Name,
                    UserRole       = UserRole.Doctor,
                    IsActive       = true,
                    CreatedAt      = DateTime.UtcNow,
                    EmailConfirmed = true
                };
                var res = await userManager.CreateAsync(user, "Doctor@123");
                if (res.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Doctor");
                    context.Doctors.Add(new Doctor
                    {
                        ApplicationUserId = user.Id,
                        FullName          = d.Name,
                        Specialization    = d.Spec,
                        ConsultationFee   = d.Fee,
                        IsAvailable       = true,
                        Phone             = d.Phone,
                        Bio               = $"Experienced specialist in {d.Spec}."
                    });
                }
            }
            await context.SaveChangesAsync();
        }

        if (!context.Patients.Any())
        {
            var patientsData = new[]
            {
                (Email: "patient1@clinic.com",  Name: "Sara Tarek",     Dob: new DateOnly(1995, 5, 20),  Gender: Gender.Female, NatId: "29505201234501", Phone: "01200000001", Blood: "A+",  Addr: "Cairo"),
                (Email: "patient2@clinic.com",  Name: "Omar Fathy",     Dob: new DateOnly(1988, 3, 15),  Gender: Gender.Male,   NatId: "28803151234502", Phone: "01200000002", Blood: "B+",  Addr: "Giza"),
                (Email: "patient3@clinic.com",  Name: "Layla Hassan",   Dob: new DateOnly(2000, 7, 10),  Gender: Gender.Female, NatId: "30007101234503", Phone: "01200000003", Blood: "O+",  Addr: "Alexandria"),
                (Email: "patient4@clinic.com",  Name: "Karim Nabil",    Dob: new DateOnly(1979, 11, 25), Gender: Gender.Male,   NatId: "27911251234504", Phone: "01200000004", Blood: "AB+", Addr: "Cairo"),
                (Email: "patient5@clinic.com",  Name: "Nadia Samir",    Dob: new DateOnly(1993, 1, 8),   Gender: Gender.Female, NatId: "29301081234505", Phone: "01200000005", Blood: "A-",  Addr: "Mansoura"),
                (Email: "patient6@clinic.com",  Name: "Youssef Adel",   Dob: new DateOnly(1985, 9, 3),   Gender: Gender.Male,   NatId: "28509031234506", Phone: "01200000006", Blood: "O-",  Addr: "Tanta"),
                (Email: "patient7@clinic.com",  Name: "Hana Mostafa",   Dob: new DateOnly(2002, 4, 17),  Gender: Gender.Female, NatId: "30204171234507", Phone: "01200000007", Blood: "B-",  Addr: "Minya"),
                (Email: "patient8@clinic.com",  Name: "Tarek Salah",    Dob: new DateOnly(1970, 6, 30),  Gender: Gender.Male,   NatId: "27006301234508", Phone: "01200000008", Blood: "A+",  Addr: "Aswan"),
                (Email: "patient9@clinic.com",  Name: "Rania Gamal",    Dob: new DateOnly(1998, 12, 5),  Gender: Gender.Female, NatId: "29812051234509", Phone: "01200000009", Blood: "O+",  Addr: "Luxor"),
                (Email: "patient10@clinic.com", Name: "Mahmoud Wael",   Dob: new DateOnly(1991, 8, 22),  Gender: Gender.Male,   NatId: "29108221234510", Phone: "01200000010", Blood: "B+",  Addr: "Suez"),
            };

            foreach (var p in patientsData)
            {
                var user = new ApplicationUser
                {
                    UserName       = p.Email,
                    Email          = p.Email,
                    FullName       = p.Name,
                    UserRole       = UserRole.Patient,
                    IsActive       = true,
                    CreatedAt      = DateTime.UtcNow,
                    EmailConfirmed = true
                };
                var res = await userManager.CreateAsync(user, "Patient@123");
                if (res.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Patient");
                    context.Patients.Add(new Patient
                    {
                        ApplicationUserId = user.Id,
                        FullName          = p.Name,
                        DateOfBirth       = p.Dob,
                        Gender            = p.Gender,
                        NationalId        = p.NatId,
                        Phone             = p.Phone,
                        BloodType         = p.Blood,
                        Address           = p.Addr,
                        CreatedAt         = DateTime.UtcNow
                    });
                }
            }
            await context.SaveChangesAsync();
        }

        if (!context.DoctorSchedules.Any())
        {
            var doctors = await context.Doctors.ToListAsync();

            var scheduleTemplates = new[]
            {
                (Day: DayOfWeek.Sunday,    Start: new TimeOnly(9,  0), End: new TimeOnly(14, 0)),
                (Day: DayOfWeek.Tuesday,   Start: new TimeOnly(14, 0), End: new TimeOnly(19, 0)),
                (Day: DayOfWeek.Monday,    Start: new TimeOnly(9,  0), End: new TimeOnly(13, 0)),
                (Day: DayOfWeek.Wednesday, Start: new TimeOnly(10, 0), End: new TimeOnly(16, 0)),
                (Day: DayOfWeek.Thursday,  Start: new TimeOnly(9,  0), End: new TimeOnly(14, 0)),
                (Day: DayOfWeek.Saturday,  Start: new TimeOnly(11, 0), End: new TimeOnly(17, 0)),
                (Day: DayOfWeek.Sunday,    Start: new TimeOnly(8,  0), End: new TimeOnly(13, 0)),
                (Day: DayOfWeek.Monday,    Start: new TimeOnly(15, 0), End: new TimeOnly(20, 0)),
            };

            for (int i = 0; i < doctors.Count; i++)
            {
                var s1 = scheduleTemplates[i * 2];
                var s2 = scheduleTemplates[i * 2 + 1];

                context.DoctorSchedules.Add(new DoctorSchedule
                {
                    DoctorId     = doctors[i].Id,
                    ScheduleType = ScheduleType.Consultation,
                    DayOfWeek    = s1.Day,
                    StartTime    = s1.Start,
                    EndTime      = s1.End,
                    SlotMinutes  = 30,
                    IsActive     = true,
                    CreatedAt    = DateTime.UtcNow
                });
                context.DoctorSchedules.Add(new DoctorSchedule
                {
                    DoctorId     = doctors[i].Id,
                    ScheduleType = ScheduleType.Consultation,
                    DayOfWeek    = s2.Day,
                    StartTime    = s2.Start,
                    EndTime      = s2.End,
                    SlotMinutes  = 30,
                    IsActive     = true,
                    CreatedAt    = DateTime.UtcNow
                });
            }
            await context.SaveChangesAsync();
        }

        if (!context.Appointments.Any())
        {
            var doctors    = await context.Doctors.ToListAsync();
            var patients   = await context.Patients.ToListAsync();
            var schedules  = await context.DoctorSchedules.ToListAsync();
            var procTypes  = await context.ProcedureTypes.ToListAsync();

            var consultType = procTypes.First(pt => pt.Name == "Consultation");
            var xrayType    = procTypes.First(pt => pt.Name == "X-Ray");
            var bloodType   = procTypes.First(pt => pt.Name == "Blood Test");
            var ecgType     = procTypes.First(pt => pt.Name == "ECG");

            var apptData = new[]
            {
                (DI: 0, PI: 0,  Days: -10, Status: AppointmentStatus.Completed, Diag: "Common Cold",            ProcType: consultType, ExtraProc: (ProcedureType?)null),
                (DI: 0, PI: 1,  Days: -8,  Status: AppointmentStatus.Completed, Diag: "Hypertension Follow-up", ProcType: consultType, ExtraProc: ecgType),
                (DI: 1, PI: 2,  Days: -7,  Status: AppointmentStatus.Completed, Diag: "Chest Pain",             ProcType: consultType, ExtraProc: ecgType),
                (DI: 1, PI: 3,  Days: -5,  Status: AppointmentStatus.Completed, Diag: "Arrhythmia",             ProcType: consultType, ExtraProc: (ProcedureType?)null),
                (DI: 2, PI: 4,  Days: -6,  Status: AppointmentStatus.Completed, Diag: "Knee Fracture",          ProcType: consultType, ExtraProc: xrayType),
                (DI: 2, PI: 5,  Days: -4,  Status: AppointmentStatus.Completed, Diag: "Lower Back Pain",        ProcType: consultType, ExtraProc: xrayType),
                (DI: 3, PI: 6,  Days: -3,  Status: AppointmentStatus.Completed, Diag: "Acne Vulgaris",          ProcType: consultType, ExtraProc: (ProcedureType?)null),
                (DI: 3, PI: 7,  Days: -2,  Status: AppointmentStatus.Completed, Diag: "Eczema",                 ProcType: consultType, ExtraProc: (ProcedureType?)null),
                (DI: 0, PI: 8,  Days: -1,  Status: AppointmentStatus.Completed, Diag: "Anemia",                 ProcType: consultType, ExtraProc: bloodType),
                (DI: 1, PI: 9,  Days: -9,  Status: AppointmentStatus.Completed, Diag: "Palpitations",           ProcType: consultType, ExtraProc: ecgType),
                (DI: 0, PI: 2,  Days: 2,   Status: AppointmentStatus.Waiting,   Diag: "",                       ProcType: consultType, ExtraProc: (ProcedureType?)null),
                (DI: 1, PI: 4,  Days: 3,   Status: AppointmentStatus.Waiting,   Diag: "",                       ProcType: consultType, ExtraProc: (ProcedureType?)null),
                (DI: 2, PI: 6,  Days: 5,   Status: AppointmentStatus.Waiting,   Diag: "",                       ProcType: consultType, ExtraProc: (ProcedureType?)null),
                (DI: 3, PI: 8,  Days: 7,   Status: AppointmentStatus.Waiting,   Diag: "",                       ProcType: consultType, ExtraProc: (ProcedureType?)null),
                (DI: 0, PI: 5,  Days: -15, Status: AppointmentStatus.Cancelled, Diag: "",                       ProcType: consultType, ExtraProc: (ProcedureType?)null),
            };

            foreach (var a in apptData)
            {
                var doctor   = doctors[a.DI];
                var patient  = patients[a.PI];
                var schedule = schedules.FirstOrDefault(s => s.DoctorId == doctor.Id);

                var appointment = new Appointment
                {
                    PatientId        = patient.Id,
                    DoctorId         = doctor.Id,
                    DoctorScheduleId = schedule?.Id,
                    AppointmentDate  = DateTime.UtcNow.AddDays(a.Days).Date.AddHours(10),
                    DurationMinutes  = 30,
                    AppointmentType  = AppointmentType.Consultation,
                    Status           = a.Status,
                    CreatedAt        = DateTime.UtcNow,
                    CancellationReason = a.Status == AppointmentStatus.Cancelled
                                        ? "Patient requested cancellation"
                                        : null
                };
                context.Appointments.Add(appointment);
                await context.SaveChangesAsync();

                if (a.Status == AppointmentStatus.Completed && !string.IsNullOrEmpty(a.Diag))
                {
                    var record = new MedicalRecord
                    {
                        PatientId     = patient.Id,
                        DoctorId      = doctor.Id,
                        AppointmentId = appointment.Id,
                        Diagnosis     = a.Diag,
                        Notes         = $"Patient presented with {a.Diag.ToLower()}. Examination conducted.",
                        VisitDate     = appointment.AppointmentDate,
                        FollowUpDate  = DateTime.UtcNow.AddDays(a.Days + 14).Date,
                        CreatedAt     = DateTime.UtcNow
                    };
                    context.MedicalRecords.Add(record);
                    await context.SaveChangesAsync();

                    record.Procedures.Add(new Procedure
                    {
                        MedicalRecordId = record.Id,
                        ProcedureTypeId = a.ProcType.Id,
                        PerformedAt     = appointment.AppointmentDate,
                        DurationMinutes = 30,
                        Cost            = a.ProcType.DefaultCost,
                        Notes           = "Routine consultation performed.",
                        CreatedAt       = DateTime.UtcNow
                    });

                    if (a.ExtraProc != null)
                    {
                        record.Procedures.Add(new Procedure
                        {
                            MedicalRecordId = record.Id,
                            ProcedureTypeId = a.ExtraProc.Id,
                            PerformedAt     = appointment.AppointmentDate,
                            DurationMinutes = 15,
                            Cost            = a.ExtraProc.DefaultCost,
                            Notes           = $"{a.ExtraProc.Name} performed as requested.",
                            AfterCareNotes  = "Follow up if symptoms persist.",
                            CreatedAt       = DateTime.UtcNow
                        });
                    }
                    await context.SaveChangesAsync();

                    var extraCost = a.ExtraProc?.DefaultCost ?? 0m;
                    var invoice = new Invoice
                    {
                        PatientId     = patient.Id,
                        AppointmentId = appointment.Id,
                        TotalAmount   = doctor.ConsultationFee + extraCost,
                        Status        = InvoiceStatus.Unpaid,
                        CreatedAt     = DateTime.UtcNow
                    };
                    context.Invoices.Add(invoice);

                    invoice.Items.Add(new InvoiceItem
                    {
                        Description = "Consultation Fee",
                        Quantity    = 1,
                        UnitPrice   = doctor.ConsultationFee,
                        ItemType    = InvoiceItemType.Consultation
                    });

                    if (a.ExtraProc != null)
                    {
                        invoice.Items.Add(new InvoiceItem
                        {
                            Description = a.ExtraProc.Name,
                            Quantity    = 1,
                            UnitPrice   = a.ExtraProc.DefaultCost,
                            ItemType    = InvoiceItemType.Procedure
                        });
                    }
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}