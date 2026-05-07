using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagementAPI.Models;

public class Instructor
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;
    
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
    
    public bool IsProfileComplete { get; set; } = false;
    
    // Foreign Key to User
    public int? UserId { get; set; }
    
    [ForeignKey("UserId")]
    public User? User { get; set; }
    
    // One-to-One relationship with InstructorProfile
    public InstructorProfile? Profile { get; set; }
    
    // One-to-Many relationship with Courses
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
