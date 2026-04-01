using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagementAPI.Models;

public class Enrollment
{
    public int Id { get; set; }
    
    // Foreign Keys for Many-to-Many relationship
    public int StudentId { get; set; }
    
    [ForeignKey("StudentId")]
    public Student Student { get; set; } = null!;
    
    public int CourseId { get; set; }
    
    [ForeignKey("CourseId")]
    public Course Course { get; set; } = null!;
    
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    
    [MaxLength(20)]
    public string Status { get; set; } = "Enrolled"; // Enrolled, Completed, Withdrawn, Failed
    
    [Range(0, 100)]
    public decimal? Grade { get; set; }
    
    [MaxLength(5)]
    public string? LetterGrade { get; set; } // A, B, C, D, F
    
    public DateTime? CompletionDate { get; set; }
}
