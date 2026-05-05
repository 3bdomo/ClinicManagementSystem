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
        var context = serviceProvider.GetRequiredService<ClinicDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Ensure database is created/migrated
        await context.Database.MigrateAsync();

        // 1. Seed Roles
        string[] roles = { "Admin", "Doctor", "Receptionist", "Patient" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Seed Admin User
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

        // 3. Seed Doctors
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

        // 4. Seed Receptionist
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
                    Phone = "01000000002"
                });
            }
        }

        // 5. Seed Procedure Types
        if (!context.ProcedureTypes.Any())
        {
            context.ProcedureTypes.AddRange(
                new ProcedureType { Name = "Consultation", Description = "General Checkup", DefaultCost = 500 },
                new ProcedureType { Name = "X-Ray", Description = "Radiology", DefaultCost = 300 },
                new ProcedureType { Name = "Blood Test", Description = "Laboratory", DefaultCost = 150 }
            );
        }

        await context.SaveChangesAsync();
    }
}
