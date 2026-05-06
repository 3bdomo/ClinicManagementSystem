using Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DAL.Context;






public class ClinicDbContextFactory : IDesignTimeDbContextFactory<ClinicDbContext>
{
    public ClinicDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ClinicDbContext>();
        
        
        
        var basePath = FindWebProjectPath() ?? Directory.GetCurrentDirectory();
        
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");

        optionsBuilder.UseSqlServer(connectionString);

        
        
        ICurrentUserService nullUserService = new NullCurrentUserService();

        return new ClinicDbContext(optionsBuilder.Options, nullUserService);
    }

    private static string? FindWebProjectPath()
    {
        
        var currentPath = Directory.GetCurrentDirectory();
        
        
        if (File.Exists(Path.Combine(currentPath, "appsettings.json")))
            return currentPath;
        
        
        for (int i = 0; i < 5; i++)
        {
            currentPath = Directory.GetParent(currentPath)?.FullName;
            if (currentPath == null) break;
            
            if (File.Exists(Path.Combine(currentPath, "appsettings.json")))
                return currentPath;
        }
        
        return null;
    }
}




internal class NullCurrentUserService : ICurrentUserService
{
    public string? UserId => null;
}



