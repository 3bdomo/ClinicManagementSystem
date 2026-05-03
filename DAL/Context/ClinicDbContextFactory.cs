using Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DAL.Context;

/// <summary>
/// Design-time DbContext factory for Entity Framework Core migrations.
/// This is used by EF Core tools (dotnet ef migrations, etc.) at design time
/// when the full dependency injection container is not available.
/// </summary>
public class ClinicDbContextFactory : IDesignTimeDbContextFactory<ClinicDbContext>
{
    public ClinicDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ClinicDbContext>();
        
        // Determine the base path for finding appsettings.json
        // Try current directory first, then try parent directories up to find the Web project
        var basePath = FindWebProjectPath() ?? Directory.GetCurrentDirectory();
        
        // Get connection string from configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");

        optionsBuilder.UseSqlServer(connectionString);

        // Create a null currentUserService for design time
        // This is only used during migrations, not in production
        ICurrentUserService nullUserService = new NullCurrentUserService();

        return new ClinicDbContext(optionsBuilder.Options, nullUserService);
    }

    private static string? FindWebProjectPath()
    {
        // Try to find the Web project folder by looking for appsettings.json
        var currentPath = Directory.GetCurrentDirectory();
        
        // Check current directory
        if (File.Exists(Path.Combine(currentPath, "appsettings.json")))
            return currentPath;
        
        // Check parent directories (up to 5 levels)
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

/// <summary>
/// Null implementation of ICurrentUserService for design-time DbContext creation.
/// </summary>
internal class NullCurrentUserService : ICurrentUserService
{
    public string? UserId => null;
}



