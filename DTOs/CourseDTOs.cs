using System.ComponentModel.DataAnnotations;

namespace CourseManagementAPI.DTOs;

// ==================== Course DTOs ====================
public class CourseCreateDto
{
    [Required(ErrorMessage = "Course code is required")]
    [MinLength(2, ErrorMessage = "Course code must be at least 2 characters")]
    [MaxLength(20, ErrorMessage = "Course code cannot exceed 20 characters")]
    public string CourseCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Title is required")]
    [MinLength(3, ErrorMessage = "Title must be at least 3 characters")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Credits is required")]
    [Range(1, 10, ErrorMessage = "Credits must be between 1 and 10")]
    public int Credits { get; set; }

    [Range(1, 500, ErrorMessage = "Max enrollment must be between 1 and 500")]
    public int MaxEnrollment { get; set; } = 30;

    [Required(ErrorMessage = "Semester is required")]
    [MaxLength(100, ErrorMessage = "Semester cannot exceed 100 characters")]
    public string Semester { get; set; } = string.Empty;

    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "End date is required")]
    public DateTime EndDate { get; set; }

    public int? InstructorId { get; set; }
}

public class CourseUpdateDto
{
    [MinLength(2, ErrorMessage = "Course code must be at least 2 characters")]
    [MaxLength(20, ErrorMessage = "Course code cannot exceed 20 characters")]
    public string? CourseCode { get; set; }

    [MinLength(3, ErrorMessage = "Title must be at least 3 characters")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string? Title { get; set; }

    [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string? Description { get; set; }

    [Range(1, 10, ErrorMessage = "Credits must be between 1 and 10")]
    public int? Credits { get; set; }

    [Range(1, 500, ErrorMessage = "Max enrollment must be between 1 and 500")]
    public int? MaxEnrollment { get; set; }

    [MaxLength(100, ErrorMessage = "Semester cannot exceed 100 characters")]
    public string? Semester { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? InstructorId { get; set; }

    public bool? IsActive { get; set; }
}

public class CourseReadDto
{
    public int Id { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Credits { get; set; }
    public int MaxEnrollment { get; set; }
    public int CurrentEnrollment { get; set; }
    public string Semester { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public InstructorSummaryDto? Instructor { get; set; }
}

public class CourseSummaryDto
{
    public int Id { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Credits { get; set; }
    public string Semester { get; set; } = string.Empty;
}
