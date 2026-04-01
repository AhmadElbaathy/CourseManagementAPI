using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagementAPI.Models;

public class Student
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
    [MaxLength(50)]
    public string StudentNumber { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? Major { get; set; }
    
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? GraduationDate { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // Foreign Key to User
    public int? UserId { get; set; }
    
    [ForeignKey("UserId")]
    public User? User { get; set; }
    
    // Many-to-Many relationship with Courses through Enrollment
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
