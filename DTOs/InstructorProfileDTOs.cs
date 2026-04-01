using System.ComponentModel.DataAnnotations;

namespace CourseManagementAPI.DTOs;

// ==================== Instructor Profile DTOs ====================
public class InstructorProfileCreateDto
{
    [Required(ErrorMessage = "Instructor ID is required")]
    public int InstructorId { get; set; }

    [MaxLength(1000, ErrorMessage = "Bio cannot exceed 1000 characters")]
    public string? Bio { get; set; }

    [MaxLength(500, ErrorMessage = "Education cannot exceed 500 characters")]
    public string? Education { get; set; }

    [MaxLength(500, ErrorMessage = "Specialization cannot exceed 500 characters")]
    public string? Specialization { get; set; }

    [MaxLength(200, ErrorMessage = "Office location cannot exceed 200 characters")]
    public string? OfficeLocation { get; set; }

    [MaxLength(100, ErrorMessage = "Office hours cannot exceed 100 characters")]
    public string? OfficeHours { get; set; }

    [Url(ErrorMessage = "Invalid website URL format")]
    [MaxLength(300, ErrorMessage = "Website URL cannot exceed 300 characters")]
    public string? Website { get; set; }

    [Url(ErrorMessage = "Invalid LinkedIn URL format")]
    [MaxLength(300, ErrorMessage = "LinkedIn URL cannot exceed 300 characters")]
    public string? LinkedInProfile { get; set; }

    [Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50")]
    public int YearsOfExperience { get; set; }
}

public class InstructorProfileUpdateDto
{
    [MaxLength(1000, ErrorMessage = "Bio cannot exceed 1000 characters")]
    public string? Bio { get; set; }

    [MaxLength(500, ErrorMessage = "Education cannot exceed 500 characters")]
    public string? Education { get; set; }

    [MaxLength(500, ErrorMessage = "Specialization cannot exceed 500 characters")]
    public string? Specialization { get; set; }

    [MaxLength(200, ErrorMessage = "Office location cannot exceed 200 characters")]
    public string? OfficeLocation { get; set; }

    [MaxLength(100, ErrorMessage = "Office hours cannot exceed 100 characters")]
    public string? OfficeHours { get; set; }

    [Url(ErrorMessage = "Invalid website URL format")]
    [MaxLength(300, ErrorMessage = "Website URL cannot exceed 300 characters")]
    public string? Website { get; set; }

    [Url(ErrorMessage = "Invalid LinkedIn URL format")]
    [MaxLength(300, ErrorMessage = "LinkedIn URL cannot exceed 300 characters")]
    public string? LinkedInProfile { get; set; }

    [Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50")]
    public int? YearsOfExperience { get; set; }
}

public class InstructorProfileReadDto
{
    public int Id { get; set; }
    public int InstructorId { get; set; }
    public string? Bio { get; set; }
    public string? Education { get; set; }
    public string? Specialization { get; set; }
    public string? OfficeLocation { get; set; }
    public string? OfficeHours { get; set; }
    public string? Website { get; set; }
    public string? LinkedInProfile { get; set; }
    public int YearsOfExperience { get; set; }
}
