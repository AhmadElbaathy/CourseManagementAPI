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
                LastLoginAt = user.LastLoginAt
            }
        };
    }

    public async Task<UserReadDto?> RegisterAsync(UserCreateDto dto)
    {
        // Check if username or email already exists
        var exists = await _context.Users
            .AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email);
        
        if (exists) return null;

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserReadDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
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
                LastLoginAt = u.LastLoginAt
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

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

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
