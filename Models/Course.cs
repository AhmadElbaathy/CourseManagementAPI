using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagementAPI.Models;

public class Course
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string CourseCode { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string? Description { get; set; }
    
    [Range(1, 10)]
    public int Credits { get; set; }
    
    [Range(1, 500)]
    public int MaxEnrollment { get; set; } = 30;
    
    [Required]
    [MaxLength(100)]
    public string Semester { get; set; } = string.Empty; // e.g., "Fall 2026"
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // One-to-Many relationship with Instructor
    public int? InstructorId { get; set; }
    
    [ForeignKey("InstructorId")]
    public Instructor? Instructor { get; set; }
    
    // Many-to-Many relationship with Students through Enrollment
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
