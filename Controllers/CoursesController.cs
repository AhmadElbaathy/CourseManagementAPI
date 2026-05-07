using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CourseManagementAPI.DTOs;
using CourseManagementAPI.Services;

namespace CourseManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly IAuthService _authService;

    public CoursesController(ICourseService courseService, IAuthService authService)
    {
        _courseService = courseService;
        _authService = authService;
    }

    /// <summary>
    /// Get all courses
    /// </summary>
    /// <returns>List of courses</returns>
    [HttpGet]
    [AllowAnonymous] // Courses can be viewed without authentication
    [ProducesResponseType(typeof(IEnumerable<CourseReadDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CourseReadDto>>> GetAll()
    {
        var courses = await _courseService.GetAllAsync();
        return Ok(courses);
    }

    /// <summary>
    /// Get a course by ID
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <returns>Course details</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CourseReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseReadDto>> GetById(int id)
    {
        var course = await _courseService.GetByIdAsync(id);
        
        if (course == null)
        {
            return NotFound(new { message = $"Course with ID {id} not found" });
        }

        return Ok(course);
    }

    /// <summary>
    /// Create a new course
    /// </summary>
    /// <param name="dto">Course data</param>
    /// <returns>Created course</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CourseReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CourseReadDto>> Create([FromBody] CourseCreateDto dto)
    {
        var course = await _courseService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = course.Id }, course);
    }

    /// <summary>
    /// Update a course
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <param name="dto">Updated course data</param>
    /// <returns>Updated course</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Instructor")]
    [ProducesResponseType(typeof(CourseReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseReadDto>> Update(int id, [FromBody] CourseUpdateDto dto)
    {
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (role == "Instructor")
        {
            var existing = await _courseService.GetByIdAsync(id);
            if (existing == null) return NotFound(new { message = $"Course with ID {id} not found" });

            int? tokenInstructorId = null;
            var instructorIdClaim = User.FindFirst("InstructorId")?.Value;
            if (instructorIdClaim != null && int.TryParse(instructorIdClaim, out int parsedId))
            {
                tokenInstructorId = parsedId;
            }
            else
            {
                // Fallback: look up InstructorId from the database using the auth user ID
                if (int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out int userId))
                {
                    var authUser = await _authService.GetUserByIdAsync(userId);
                    tokenInstructorId = authUser?.InstructorId;
                }
            }

            if (tokenInstructorId == null || existing.Instructor?.Id != tokenInstructorId.Value)
            {
                return Forbid();
            }
        }

        var course = await _courseService.UpdateAsync(id, dto);
        
        if (course == null)
        {
            return NotFound(new { message = $"Course with ID {id} not found" });
        }

        return Ok(course);
    }

    /// <summary>
    /// Delete a course
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _courseService.DeleteAsync(id);
        
        if (!result)
        {
            return NotFound(new { message = $"Course with ID {id} not found" });
        }

        return NoContent();
    }

    /// <summary>
    /// Get courses by instructor
    /// </summary>
    /// <param name="instructorId">Instructor ID</param>
    /// <returns>List of courses</returns>
    [HttpGet("instructor/{instructorId}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<CourseReadDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CourseReadDto>>> GetByInstructor(int instructorId)
    {
        var courses = await _courseService.GetByInstructorAsync(instructorId);
        return Ok(courses);
    }

    /// <summary>
    /// Get enrolled students for a course
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <returns>List of enrolled students</returns>
    [HttpGet("{id}/students")]
    [Authorize(Roles = "Admin,Instructor")]
    [ProducesResponseType(typeof(IEnumerable<StudentSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<StudentSummaryDto>>> GetEnrolledStudents(int id)
    {
        // Verify course exists
        var course = await _courseService.GetByIdAsync(id);
        if (course == null)
        {
            return NotFound(new { message = $"Course with ID {id} not found" });
        }

        var students = await _courseService.GetEnrolledStudentsAsync(id);
        return Ok(students);
    }
}
