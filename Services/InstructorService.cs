using Microsoft.EntityFrameworkCore;
using CourseManagementAPI.Data;
using CourseManagementAPI.DTOs;
using CourseManagementAPI.Models;

namespace CourseManagementAPI.Services;

public interface IInstructorService
{
    Task<IEnumerable<InstructorReadDto>> GetAllAsync();
    Task<InstructorReadDto?> GetByIdAsync(int id);
    Task<InstructorReadDto> CreateAsync(InstructorCreateDto dto);
    Task<InstructorReadDto?> UpdateAsync(int id, InstructorUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<InstructorProfileReadDto?> GetProfileAsync(int instructorId);
    Task<InstructorProfileReadDto> CreateOrUpdateProfileAsync(int instructorId, InstructorProfileCreateDto dto);
    Task<InstructorReadDto?> GetByUserIdAsync(int userId);
}

public class InstructorService : IInstructorService
{
    private readonly ApplicationDbContext _context;

    public InstructorService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InstructorReadDto>> GetAllAsync()
    {
        // Using Select() projection and AsNoTracking() for optimization
        return await _context.Instructors
            .AsNoTracking()
            .Select(i => new InstructorReadDto
            {
                Id = i.Id,
                FirstName = i.FirstName,
                LastName = i.LastName,
                Email = i.Email,
                Phone = i.Phone,
                Department = i.Department,
                HireDate = i.HireDate,
                IsActive = i.IsActive,
                IsProfileComplete = i.IsProfileComplete,
                CourseCount = i.Courses.Count,
                Profile = i.Profile != null ? new InstructorProfileReadDto
                {
                    Id = i.Profile.Id,
                    InstructorId = i.Profile.InstructorId,
                    Bio = i.Profile.Bio,
                    Education = i.Profile.Education,
                    Specialization = i.Profile.Specialization,
                    OfficeLocation = i.Profile.OfficeLocation,
                    OfficeHours = i.Profile.OfficeHours,
                    Website = i.Profile.Website,
                    LinkedInProfile = i.Profile.LinkedInProfile,
                    YearsOfExperience = i.Profile.YearsOfExperience
                } : null
            })
            .ToListAsync();
    }

    public async Task<InstructorReadDto?> GetByIdAsync(int id)
    {
        return await _context.Instructors
            .AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => new InstructorReadDto
            {
                Id = i.Id,
                FirstName = i.FirstName,
                LastName = i.LastName,
                Email = i.Email,
                Phone = i.Phone,
                Department = i.Department,
                HireDate = i.HireDate,
                IsActive = i.IsActive,
                IsProfileComplete = i.IsProfileComplete,
                CourseCount = i.Courses.Count,
                Profile = i.Profile != null ? new InstructorProfileReadDto
                {
                    Id = i.Profile.Id,
                    InstructorId = i.Profile.InstructorId,
                    Bio = i.Profile.Bio,
                    Education = i.Profile.Education,
                    Specialization = i.Profile.Specialization,
                    OfficeLocation = i.Profile.OfficeLocation,
                    OfficeHours = i.Profile.OfficeHours,
                    Website = i.Profile.Website,
                    LinkedInProfile = i.Profile.LinkedInProfile,
                    YearsOfExperience = i.Profile.YearsOfExperience
                } : null
            })
            .FirstOrDefaultAsync();
    }

    public async Task<InstructorReadDto> CreateAsync(InstructorCreateDto dto)
    {
        var instructor = new Instructor
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Department = dto.Department,
            HireDate = dto.HireDate ?? DateTime.UtcNow,
            IsActive = true,
            IsProfileComplete = true
        };

        _context.Instructors.Add(instructor);
        await _context.SaveChangesAsync();

        return new InstructorReadDto
        {
            Id = instructor.Id,
            FirstName = instructor.FirstName,
            LastName = instructor.LastName,
            Email = instructor.Email,
            Phone = instructor.Phone,
            Department = instructor.Department,
            HireDate = instructor.HireDate,
            IsActive = instructor.IsActive,
            IsProfileComplete = instructor.IsProfileComplete,
            CourseCount = 0
        };
    }

    public async Task<InstructorReadDto?> UpdateAsync(int id, InstructorUpdateDto dto)
    {
        var instructor = await _context.Instructors.FindAsync(id);
        if (instructor == null) return null;

        if (dto.FirstName != null) instructor.FirstName = dto.FirstName;
        if (dto.LastName != null) instructor.LastName = dto.LastName;
        if (dto.Email != null) instructor.Email = dto.Email;
        if (dto.Phone != null) instructor.Phone = dto.Phone;
        if (dto.Department != null) instructor.Department = dto.Department;
        if (dto.IsActive.HasValue) instructor.IsActive = dto.IsActive.Value;

        instructor.IsProfileComplete = true; // Mark as complete after update
        
        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var instructor = await _context.Instructors.FindAsync(id);
        if (instructor == null) return false;

        _context.Instructors.Remove(instructor);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<InstructorProfileReadDto?> GetProfileAsync(int instructorId)
    {
        return await _context.InstructorProfiles
            .AsNoTracking()
            .Where(p => p.InstructorId == instructorId)
            .Select(p => new InstructorProfileReadDto
            {
                Id = p.Id,
                InstructorId = p.InstructorId,
                Bio = p.Bio,
                Education = p.Education,
                Specialization = p.Specialization,
                OfficeLocation = p.OfficeLocation,
                OfficeHours = p.OfficeHours,
                Website = p.Website,
                LinkedInProfile = p.LinkedInProfile,
                YearsOfExperience = p.YearsOfExperience
            })
            .FirstOrDefaultAsync();
    }

    public async Task<InstructorProfileReadDto> CreateOrUpdateProfileAsync(int instructorId, InstructorProfileCreateDto dto)
    {
        var profile = await _context.InstructorProfiles
            .FirstOrDefaultAsync(p => p.InstructorId == instructorId);

        if (profile == null)
        {
            profile = new InstructorProfile
            {
                InstructorId = instructorId,
                Bio = dto.Bio,
                Education = dto.Education,
                Specialization = dto.Specialization,
                OfficeLocation = dto.OfficeLocation,
                OfficeHours = dto.OfficeHours,
                Website = dto.Website,
                LinkedInProfile = dto.LinkedInProfile,
                YearsOfExperience = dto.YearsOfExperience
            };
            _context.InstructorProfiles.Add(profile);
        }
        else
        {
            profile.Bio = dto.Bio;
            profile.Education = dto.Education;
            profile.Specialization = dto.Specialization;
            profile.OfficeLocation = dto.OfficeLocation;
            profile.OfficeHours = dto.OfficeHours;
            profile.Website = dto.Website;
            profile.LinkedInProfile = dto.LinkedInProfile;
            profile.YearsOfExperience = dto.YearsOfExperience;
        }

        await _context.SaveChangesAsync();

        return new InstructorProfileReadDto
        {
            Id = profile.Id,
            InstructorId = profile.InstructorId,
            Bio = profile.Bio,
            Education = profile.Education,
            Specialization = profile.Specialization,
            OfficeLocation = profile.OfficeLocation,
            OfficeHours = profile.OfficeHours,
            Website = profile.Website,
            LinkedInProfile = profile.LinkedInProfile,
            YearsOfExperience = profile.YearsOfExperience
        };
    }

    public async Task<InstructorReadDto?> GetByUserIdAsync(int userId)
    {
        return await _context.Instructors
            .AsNoTracking()
            .Where(i => i.UserId == userId)
            .Select(i => new InstructorReadDto
            {
                Id = i.Id,
                FirstName = i.FirstName,
                LastName = i.LastName,
                Email = i.Email,
                Phone = i.Phone,
                Department = i.Department,
                HireDate = i.HireDate,
                IsActive = i.IsActive,
                IsProfileComplete = i.IsProfileComplete,
                CourseCount = i.Courses.Count
            })
            .FirstOrDefaultAsync();
    }
}
