using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagementAPI.Models;

public class RefreshToken
{
    public int Id { get; set; }
    
    [Required]
    public string Token { get; set; } = string.Empty;
    
    public DateTime ExpiresAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? RevokedAt { get; set; }
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    
    public bool IsRevoked => RevokedAt != null;
    
    public bool IsActive => !IsRevoked && !IsExpired;
    
    // Foreign Key to User
    public int UserId { get; set; }
    
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}
