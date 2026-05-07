using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CourseManagementAPI.Data;
using CourseManagementAPI.DTOs;
using CourseManagementAPI.Models;

namespace CourseManagementAPI.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto dto);
    Task<UserReadDto?> RegisterAsync(UserCreateDto dto);
    Task<UserReadDto?> GetUserByIdAsync(int id);
    Task<TokenResponseDto?> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeTokenAsync(string refreshToken);
}

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == dto.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return null;
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;

        // Generate tokens
        var token = GenerateJwtToken(user);
        var refreshToken = await GenerateRefreshTokenAsync(user.Id);
        
        await _context.SaveChangesAsync();

        var expiration = DateTime.UtcNow.AddHours(
            double.Parse(_configuration["Jwt:ExpirationHours"] ?? "24"));

        bool isProfileComplete = false;
        int? studentId = null;
        int? instructorId = null;

        if (user.Role == "Student")
        {
            var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == user.Id);
            studentId = student?.Id;
            isProfileComplete = student?.IsProfileComplete ?? false;
        }
        else if (user.Role == "Instructor")
        {
            var instructor = await _context.Instructors.AsNoTracking().FirstOrDefaultAsync(i => i.UserId == user.Id);
            instructorId = instructor?.Id;
            isProfileComplete = instructor?.IsProfileComplete ?? false;
        }
        else if (user.Role == "Admin")
        {
            isProfileComplete = true;
        }

        return new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken.Token,
            Expiration = expiration,
            User = new UserReadDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                StudentId = studentId,
                InstructorId = instructorId,
                IsProfileComplete = isProfileComplete
            }
        };
    }

    public async Task<UserReadDto?> RegisterAsync(UserCreateDto dto)
    {
        // Check if username or email already exists in any table
        var userExists = await _context.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email);
        var studentExists = await _context.Students.AnyAsync(s => s.Email == dto.Email);
        var instructorExists = await _context.Instructors.AnyAsync(i => i.Email == dto.Email);
        
        if (userExists || studentExists || instructorExists) return null;

        // Determine role from password suffix, default to Student
        var role = "Student";

        if (dto.Password.EndsWith("_SecretAdmin"))
        {
            role = "Admin";
        }
        else if (dto.Password.EndsWith("_SecretInstructor"))
        {
            role = "Instructor";
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = role,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            int? studentId = null;
            int? instructorId = null;

            if (role == "Student")
            {
                var student = new Student
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = "New",
                    LastName = "Student",
                    StudentNumber = $"S{DateTime.UtcNow.Year}{user.Id.ToString("D4")}",
                    IsActive = true
                };
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                studentId = student.Id;
            }
            else if (role == "Instructor")
            {
                var instructor = new Instructor
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = "New",
                    LastName = "Instructor",
                    Department = "Pending",
                    IsActive = true
                };
                _context.Instructors.Add(instructor);
                await _context.SaveChangesAsync();
                instructorId = instructor.Id;
            }

            await transaction.CommitAsync();

            return new UserReadDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                StudentId = studentId,
                InstructorId = instructorId
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<UserReadDto?> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserReadDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt,
                StudentId = _context.Students.Where(s => s.UserId == u.Id).Select(s => (int?)s.Id).FirstOrDefault(),
                InstructorId = _context.Instructors.Where(i => i.UserId == u.Id).Select(i => (int?)i.Id).FirstOrDefault(),
                IsProfileComplete = u.Role == "Admin" || 
                                   (u.Role == "Student" && _context.Students.Any(s => s.UserId == u.Id && s.IsProfileComplete)) ||
                                   (u.Role == "Instructor" && _context.Instructors.Any(i => i.UserId == u.Id && i.IsProfileComplete))
            })
            .FirstOrDefaultAsync();
    }

    public async Task<TokenResponseDto?> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken == null || !storedToken.IsActive)
        {
            return null;
        }

        // Revoke old refresh token
        storedToken.RevokedAt = DateTime.UtcNow;

        // Generate new tokens
        var newJwtToken = GenerateJwtToken(storedToken.User);
        var newRefreshToken = await GenerateRefreshTokenAsync(storedToken.UserId);

        await _context.SaveChangesAsync();

        var expiration = DateTime.UtcNow.AddHours(
            double.Parse(_configuration["Jwt:ExpirationHours"] ?? "24"));

        return new TokenResponseDto
        {
            Token = newJwtToken,
            RefreshToken = newRefreshToken.Token,
            Expiration = expiration
        };
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken == null || !storedToken.IsActive)
        {
            return false;
        }

        storedToken.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        if (user.Role == "Student")
        {
            var student = _context.Students.AsNoTracking().FirstOrDefault(s => s.UserId == user.Id);
            if (student != null)
            {
                claims.Add(new Claim("StudentId", student.Id.ToString()));
            }
        }
        else if (user.Role == "Instructor")
        {
            var instructor = _context.Instructors.AsNoTracking().FirstOrDefault(i => i.UserId == user.Id);
            if (instructor != null)
            {
                claims.Add(new Claim("InstructorId", instructor.Id.ToString()));
            }
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(
                double.Parse(_configuration["Jwt:ExpirationHours"] ?? "24")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<RefreshToken> GenerateRefreshTokenAsync(int userId)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            ExpiresAt = DateTime.UtcNow.AddDays(7), // Refresh token valid for 7 days
            UserId = userId
        };

        _context.RefreshTokens.Add(refreshToken);
        return refreshToken;
    }
}
