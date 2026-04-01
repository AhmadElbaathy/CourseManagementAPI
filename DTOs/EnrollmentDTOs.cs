using System.ComponentModel.DataAnnotations;

namespace CourseManagementAPI.DTOs;

// ==================== Enrollment DTOs ====================
public class EnrollmentCreateDto
{
    [Required(ErrorMessage = "Student ID is required")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Course ID is required")]
    public int CourseId { get; set; }
}

public class EnrollmentUpdateDto
{
    [MaxLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
    [RegularExpression("^(Enrolled|Completed|Withdrawn|Failed)$", 
        ErrorMessage = "Status must be one of: Enrolled, Completed, Withdrawn, Failed")]
    public string? Status { get; set; }

    [Range(0, 100, ErrorMessage = "Grade must be between 0 and 100")]
    public decimal? Grade { get; set; }

    [MaxLength(5, ErrorMessage = "Letter grade cannot exceed 5 characters")]
    [RegularExpression("^(A\\+?|B\\+?|C\\+?|D\\+?|F)$", 
        ErrorMessage = "Letter grade must be a valid grade (A, B, C, D, F with optional +)")]
    public string? LetterGrade { get; set; }

    public DateTime? CompletionDate { get; set; }
}

public class EnrollmentReadDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? Grade { get; set; }
    public string? LetterGrade { get; set; }
    public DateTime? CompletionDate { get; set; }
    public StudentSummaryDto Student { get; set; } = null!;
    public CourseSummaryDto Course { get; set; } = null!;
}

public class EnrollmentSummaryDto
{
    public int Id { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal? Grade { get; set; }
    public string? LetterGrade { get; set; }
}
