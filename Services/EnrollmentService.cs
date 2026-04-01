using Microsoft.EntityFrameworkCore;
using CourseManagementAPI.Data;
using CourseManagementAPI.DTOs;
using CourseManagementAPI.Models;

namespace CourseManagementAPI.Services;

public interface IEnrollmentService
{
    Task<IEnumerable<EnrollmentReadDto>> GetAllAsync();
    Task<EnrollmentReadDto?> GetByIdAsync(int id);
    Task<EnrollmentReadDto?> CreateAsync(EnrollmentCreateDto dto);
    Task<EnrollmentReadDto?> UpdateAsync(int id, EnrollmentUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> IsStudentEnrolledAsync(int studentId, int courseId);
}

public class EnrollmentService : IEnrollmentService
{
    private readonly ApplicationDbContext _context;

    public EnrollmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EnrollmentReadDto>> GetAllAsync()
    {
        // Using Select() projection and AsNoTracking() for optimization
        return await _context.Enrollments
            .AsNoTracking()
            .Select(e => new EnrollmentReadDto
            {
                Id = e.Id,
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                EnrollmentDate = e.EnrollmentDate,
                Status = e.Status,
                Grade = e.Grade,
                LetterGrade = e.LetterGrade,
                CompletionDate = e.CompletionDate,
                Student = new StudentSummaryDto
                {
                    Id = e.Student.Id,
                    FullName = e.Student.FirstName + " " + e.Student.LastName,
                    StudentNumber = e.Student.StudentNumber,
                    Email = e.Student.Email
                },
                Course = new CourseSummaryDto
                {
                    Id = e.Course.Id,
                    CourseCode = e.Course.CourseCode,
                    Title = e.Course.Title,
                    Credits = e.Course.Credits,
                    Semester = e.Course.Semester
                }
            })
            .ToListAsync();
    }

    public async Task<EnrollmentReadDto?> GetByIdAsync(int id)
    {
        return await _context.Enrollments
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EnrollmentReadDto
            {
                Id = e.Id,
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                EnrollmentDate = e.EnrollmentDate,
                Status = e.Status,
                Grade = e.Grade,
                LetterGrade = e.LetterGrade,
                CompletionDate = e.CompletionDate,
                Student = new StudentSummaryDto
                {
                    Id = e.Student.Id,
                    FullName = e.Student.FirstName + " " + e.Student.LastName,
                    StudentNumber = e.Student.StudentNumber,
                    Email = e.Student.Email
                },
                Course = new CourseSummaryDto
                {
                    Id = e.Course.Id,
                    CourseCode = e.Course.CourseCode,
                    Title = e.Course.Title,
                    Credits = e.Course.Credits,
                    Semester = e.Course.Semester
                }
            })
            .FirstOrDefaultAsync();
    }

    public async Task<EnrollmentReadDto?> CreateAsync(EnrollmentCreateDto dto)
    {
        // Check if student exists
        var studentExists = await _context.Students.AnyAsync(s => s.Id == dto.StudentId);
        if (!studentExists) return null;

        // Check if course exists
        var course = await _context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == dto.CourseId);
        if (course == null) return null;

        // Check if already enrolled
        var alreadyEnrolled = await IsStudentEnrolledAsync(dto.StudentId, dto.CourseId);
        if (alreadyEnrolled) return null;

        // Check if course has space
        var currentEnrollment = await _context.Enrollments
            .CountAsync(e => e.CourseId == dto.CourseId && e.Status == "Enrolled");
        if (currentEnrollment >= course.MaxEnrollment) return null;

        var enrollment = new Enrollment
        {
            StudentId = dto.StudentId,
            CourseId = dto.CourseId,
            EnrollmentDate = DateTime.UtcNow,
            Status = "Enrolled"
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(enrollment.Id);
    }

    public async Task<EnrollmentReadDto?> UpdateAsync(int id, EnrollmentUpdateDto dto)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);
        if (enrollment == null) return null;

        if (dto.Status != null) enrollment.Status = dto.Status;
        if (dto.Grade.HasValue) enrollment.Grade = dto.Grade;
        if (dto.LetterGrade != null) enrollment.LetterGrade = dto.LetterGrade;
        if (dto.CompletionDate.HasValue) enrollment.CompletionDate = dto.CompletionDate;

        // Auto-set completion date when status is Completed
        if (dto.Status == "Completed" && !enrollment.CompletionDate.HasValue)
        {
            enrollment.CompletionDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);
        if (enrollment == null) return false;

        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsStudentEnrolledAsync(int studentId, int courseId)
    {
        return await _context.Enrollments
            .AsNoTracking()
            .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);
    }
}
