using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagementAPI.Models;

public class InstructorProfile
{
    public int Id { get; set; }
    
    // Foreign Key - One-to-One with Instructor
    public int InstructorId { get; set; }
    
    [ForeignKey("InstructorId")]
    public Instructor Instructor { get; set; } = null!;
    
    [MaxLength(1000)]
    public string? Bio { get; set; }
    
    [MaxLength(500)]
    public string? Education { get; set; }
    
    [MaxLength(500)]
    public string? Specialization { get; set; }
    
    [MaxLength(200)]
    public string? OfficeLocation { get; set; }
    
    [MaxLength(100)]
    public string? OfficeHours { get; set; }
    
    [MaxLength(300)]
    public string? Website { get; set; }
    
    [MaxLength(300)]
    public string? LinkedInProfile { get; set; }
    
    public int YearsOfExperience { get; set; }
}
