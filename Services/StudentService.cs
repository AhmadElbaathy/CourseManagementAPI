using Microsoft.EntityFrameworkCore;
using CourseManagementAPI.Data;
using CourseManagementAPI.DTOs;
using CourseManagementAPI.Models;

namespace CourseManagementAPI.Services;

public interface IStudentService
{
    Task<IEnumerable<StudentReadDto>> GetAllAsync();
    Task<StudentReadDto?> GetByIdAsync(int id);
    Task<StudentReadDto> CreateAsync(StudentCreateDto dto);
    Task<StudentReadDto?> UpdateAsync(int id, StudentUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<EnrollmentSummaryDto>> GetEnrollmentsAsync(int studentId);
}

public class StudentService : IStudentService
{
    private readonly ApplicationDbContext _context;

    public StudentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StudentReadDto>> GetAllAsync()
    {
        // Using Select() projection and AsNoTracking() for optimization
        return await _context.Students
            .AsNoTracking()
            .Select(s => new StudentReadDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                Phone = s.Phone,
                StudentNumber = s.StudentNumber,
                Major = s.Major,
                EnrollmentDate = s.EnrollmentDate,
                GraduationDate = s.GraduationDate,
                IsActive = s.IsActive,
                IsProfileComplete = s.IsProfileComplete,
                EnrolledCoursesCount = s.Enrollments.Count(e => e.Status == "Enrolled")
            })
            .ToListAsync();
    }

    public async Task<StudentReadDto?> GetByIdAsync(int id)
    {
        return await _context.Students
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new StudentReadDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                Phone = s.Phone,
                StudentNumber = s.StudentNumber,
                Major = s.Major,
                EnrollmentDate = s.EnrollmentDate,
                GraduationDate = s.GraduationDate,
                IsActive = s.IsActive,
                IsProfileComplete = s.IsProfileComplete,
                EnrolledCoursesCount = s.Enrollments.Count(e => e.Status == "Enrolled")
            })
            .FirstOrDefaultAsync();
    }

    public async Task<StudentReadDto> CreateAsync(StudentCreateDto dto)
    {
        var student = new Student
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            StudentNumber = dto.StudentNumber,
            Major = dto.Major,
            EnrollmentDate = DateTime.UtcNow,
            IsActive = true,
            IsProfileComplete = true
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return new StudentReadDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            Phone = student.Phone,
            StudentNumber = student.StudentNumber,
            Major = student.Major,
            EnrollmentDate = student.EnrollmentDate,
            IsActive = student.IsActive,
            IsProfileComplete = student.IsProfileComplete,
            EnrolledCoursesCount = 0
        };
    }

    public async Task<StudentReadDto?> UpdateAsync(int id, StudentUpdateDto dto)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) return null;

        if (dto.FirstName != null) student.FirstName = dto.FirstName;
        if (dto.LastName != null) student.LastName = dto.LastName;
        if (dto.Email != null) student.Email = dto.Email;
        if (dto.Phone != null) student.Phone = dto.Phone;
        if (dto.Major != null) student.Major = dto.Major;
        if (dto.GraduationDate.HasValue) student.GraduationDate = dto.GraduationDate;
        if (dto.IsActive.HasValue) student.IsActive = dto.IsActive.Value;

        student.IsProfileComplete = true; // Mark as complete after update

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) return false;

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<EnrollmentSummaryDto>> GetEnrollmentsAsync(int studentId)
    {
        return await _context.Enrollments
            .AsNoTracking()
            .Where(e => e.StudentId == studentId)
            .Select(e => new EnrollmentSummaryDto
            {
                Id = e.Id,
                CourseName = $"{e.Course.CourseCode} - {e.Course.Title}",
                Status = e.Status,
                Grade = e.Grade,
                LetterGrade = e.LetterGrade
            })
            .ToListAsync();
    }
}
