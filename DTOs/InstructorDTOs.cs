using System.ComponentModel.DataAnnotations;

namespace CourseManagementAPI.DTOs;

// ==================== Instructor DTOs ====================
public class InstructorCreateDto
{
    [Required(ErrorMessage = "First name is required")]
    [MinLength(2, ErrorMessage = "First name must be at least 2 characters")]
    [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [MinLength(2, ErrorMessage = "Last name must be at least 2 characters")]
    [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Department is required")]
    [MaxLength(100, ErrorMessage = "Department cannot exceed 100 characters")]
    public string Department { get; set; } = string.Empty;

    public DateTime? HireDate { get; set; }
}

public class InstructorUpdateDto
{
    [MinLength(2, ErrorMessage = "First name must be at least 2 characters")]
    [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    public string? FirstName { get; set; }

    [MinLength(2, ErrorMessage = "Last name must be at least 2 characters")]
    [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    public string? LastName { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? Phone { get; set; }

    [MaxLength(100, ErrorMessage = "Department cannot exceed 100 characters")]
    public string? Department { get; set; }

    public bool? IsActive { get; set; }
}

public class InstructorReadDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Department { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsProfileComplete { get; set; }
    public int CourseCount { get; set; }
    public InstructorProfileReadDto? Profile { get; set; }
}

public class InstructorSummaryDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}
