using Microsoft.EntityFrameworkCore;
using CourseManagementAPI.Data;

namespace CourseManagementAPI.Services;

public interface IBackgroundJobService
{
    Task CleanupExpiredRefreshTokensAsync();
    Task GenerateDailyEnrollmentReportAsync();
    Task DeactivateOldEnrollmentsAsync();
}

public class BackgroundJobService : IBackgroundJobService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundJobService> _logger;

    public BackgroundJobService(IServiceProvider serviceProvider, ILogger<BackgroundJobService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Cleans up expired and revoked refresh tokens from the database
    /// Runs daily to keep the database clean
    /// </summary>
    public async Task CleanupExpiredRefreshTokensAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var expiredTokens = await context.RefreshTokens
            .Where(rt => rt.ExpiresAt < DateTime.UtcNow || rt.RevokedAt != null)
            .ToListAsync();

        if (expiredTokens.Any())
        {
            context.RefreshTokens.RemoveRange(expiredTokens);
            await context.SaveChangesAsync();
            _logger.LogInformation("Cleaned up {Count} expired/revoked refresh tokens", expiredTokens.Count);
        }
        else
        {
            _logger.LogInformation("No expired refresh tokens to clean up");
        }
    }

    /// <summary>
    /// Generates a daily enrollment report
    /// Logs statistics about enrollments
    /// </summary>
    public async Task GenerateDailyEnrollmentReportAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var totalStudents = await context.Students.CountAsync();
        var totalCourses = await context.Courses.CountAsync();
        var totalEnrollments = await context.Enrollments.CountAsync();
        var activeEnrollments = await context.Enrollments.CountAsync(e => e.Status == "Enrolled");
        var completedEnrollments = await context.Enrollments.CountAsync(e => e.Status == "Completed");

        _logger.LogInformation(
            "Daily Report - Students: {Students}, Courses: {Courses}, " +
            "Total Enrollments: {Total}, Active: {Active}, Completed: {Completed}",
            totalStudents, totalCourses, totalEnrollments, activeEnrollments, completedEnrollments);
    }

    /// <summary>
    /// Marks old enrollments as completed if the course end date has passed
    /// </summary>
    public async Task DeactivateOldEnrollmentsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var oldEnrollments = await context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.Status == "Enrolled" && e.Course.EndDate < DateTime.UtcNow)
            .ToListAsync();

        foreach (var enrollment in oldEnrollments)
        {
            enrollment.Status = "Completed";
            enrollment.CompletionDate = enrollment.Course.EndDate;
        }

        if (oldEnrollments.Any())
        {
            await context.SaveChangesAsync();
            _logger.LogInformation("Auto-completed {Count} enrollments for finished courses", oldEnrollments.Count);
        }
    }
}
