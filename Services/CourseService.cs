using Microsoft.EntityFrameworkCore;
using CourseManagementAPI.Data;
using CourseManagementAPI.DTOs;
using CourseManagementAPI.Models;

namespace CourseManagementAPI.Services;

public interface ICourseService
{
    Task<IEnumerable<CourseReadDto>> GetAllAsync();
    Task<CourseReadDto?> GetByIdAsync(int id);
    Task<CourseReadDto> CreateAsync(CourseCreateDto dto);
    Task<CourseReadDto?> UpdateAsync(int id, CourseUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<CourseReadDto>> GetByInstructorAsync(int instructorId);
    Task<IEnumerable<StudentSummaryDto>> GetEnrolledStudentsAsync(int courseId);
}

public class CourseService : ICourseService
{
    private readonly ApplicationDbContext _context;

    public CourseService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CourseReadDto>> GetAllAsync()
    {
        // Using Select() projection and AsNoTracking() for optimization
        return await _context.Courses
            .AsNoTracking()
            .Select(c => new CourseReadDto
            {
                Id = c.Id,
                CourseCode = c.CourseCode,
                Title = c.Title,
                Description = c.Description,
                Credits = c.Credits,
                MaxEnrollment = c.MaxEnrollment,
                CurrentEnrollment = c.Enrollments.Count(e => e.Status == "Enrolled"),
                Semester = c.Semester,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                IsActive = c.IsActive,
                Instructor = c.Instructor != null ? new InstructorSummaryDto
                {
                    Id = c.Instructor.Id,
                    FullName = c.Instructor.FirstName + " " + c.Instructor.LastName,
                    Email = c.Instructor.Email,
                    Department = c.Instructor.Department
                } : null
            })
            .ToListAsync();
    }

    public async Task<CourseReadDto?> GetByIdAsync(int id)
    {
        return await _context.Courses
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CourseReadDto
            {
                Id = c.Id,
                CourseCode = c.CourseCode,
                Title = c.Title,
                Description = c.Description,
                Credits = c.Credits,
                MaxEnrollment = c.MaxEnrollment,
                CurrentEnrollment = c.Enrollments.Count(e => e.Status == "Enrolled"),
                Semester = c.Semester,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                IsActive = c.IsActive,
                Instructor = c.Instructor != null ? new InstructorSummaryDto
                {
                    Id = c.Instructor.Id,
                    FullName = c.Instructor.FirstName + " " + c.Instructor.LastName,
                    Email = c.Instructor.Email,
                    Department = c.Instructor.Department
                } : null
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CourseReadDto> CreateAsync(CourseCreateDto dto)
    {
        var course = new Course
        {
            CourseCode = dto.CourseCode,
            Title = dto.Title,
            Description = dto.Description,
            Credits = dto.Credits,
            MaxEnrollment = dto.MaxEnrollment,
            Semester = dto.Semester,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            InstructorId = dto.InstructorId,
            IsActive = true
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(course.Id))!;
    }

    public async Task<CourseReadDto?> UpdateAsync(int id, CourseUpdateDto dto)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return null;

        if (dto.CourseCode != null) course.CourseCode = dto.CourseCode;
        if (dto.Title != null) course.Title = dto.Title;
        if (dto.Description != null) course.Description = dto.Description;
        if (dto.Credits.HasValue) course.Credits = dto.Credits.Value;
        if (dto.MaxEnrollment.HasValue) course.MaxEnrollment = dto.MaxEnrollment.Value;
        if (dto.Semester != null) course.Semester = dto.Semester;
        if (dto.StartDate.HasValue) course.StartDate = dto.StartDate.Value;
        if (dto.EndDate.HasValue) course.EndDate = dto.EndDate.Value;
        if (dto.InstructorId.HasValue) course.InstructorId = dto.InstructorId;
        if (dto.IsActive.HasValue) course.IsActive = dto.IsActive.Value;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return false;

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<CourseReadDto>> GetByInstructorAsync(int instructorId)
    {
        return await _context.Courses
            .AsNoTracking()
            .Where(c => c.InstructorId == instructorId)
            .Select(c => new CourseReadDto
            {
                Id = c.Id,
                CourseCode = c.CourseCode,
                Title = c.Title,
                Description = c.Description,
                Credits = c.Credits,
                MaxEnrollment = c.MaxEnrollment,
                CurrentEnrollment = c.Enrollments.Count(e => e.Status == "Enrolled"),
                Semester = c.Semester,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                IsActive = c.IsActive,
                Instructor = c.Instructor != null ? new InstructorSummaryDto
                {
                    Id = c.Instructor.Id,
                    FullName = c.Instructor.FirstName + " " + c.Instructor.LastName,
                    Email = c.Instructor.Email,
                    Department = c.Instructor.Department
                } : null
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<StudentSummaryDto>> GetEnrolledStudentsAsync(int courseId)
    {
        return await _context.Enrollments
            .AsNoTracking()
            .Where(e => e.CourseId == courseId && e.Status == "Enrolled")
            .Select(e => new StudentSummaryDto
            {
                Id = e.Student.Id,
                FullName = e.Student.FirstName + " " + e.Student.LastName,
                StudentNumber = e.Student.StudentNumber,
                Email = e.Student.Email
            })
            .ToListAsync();
    }
}
