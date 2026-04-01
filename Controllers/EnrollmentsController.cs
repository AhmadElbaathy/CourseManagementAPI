using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CourseManagementAPI.DTOs;
using CourseManagementAPI.Services;

namespace CourseManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    /// <summary>
    /// Get all enrollments
    /// </summary>
    /// <returns>List of enrollments</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,Instructor")]
    [ProducesResponseType(typeof(IEnumerable<EnrollmentReadDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EnrollmentReadDto>>> GetAll()
    {
        var enrollments = await _enrollmentService.GetAllAsync();
        return Ok(enrollments);
    }

    /// <summary>
    /// Get an enrollment by ID
    /// </summary>
    /// <param name="id">Enrollment ID</param>
    /// <returns>Enrollment details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EnrollmentReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EnrollmentReadDto>> GetById(int id)
    {
        var enrollment = await _enrollmentService.GetByIdAsync(id);
        
        if (enrollment == null)
        {
            return NotFound(new { message = $"Enrollment with ID {id} not found" });
        }

        return Ok(enrollment);
    }

    /// <summary>
    /// Enroll a student in a course
    /// </summary>
    /// <param name="dto">Enrollment data</param>
    /// <returns>Created enrollment</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Student")]
    [ProducesResponseType(typeof(EnrollmentReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EnrollmentReadDto>> Create([FromBody] EnrollmentCreateDto dto)
    {
        // Check if already enrolled
        var isEnrolled = await _enrollmentService.IsStudentEnrolledAsync(dto.StudentId, dto.CourseId);
        if (isEnrolled)
        {
            return Conflict(new { message = "Student is already enrolled in this course" });
        }

        var enrollment = await _enrollmentService.CreateAsync(dto);
        
        if (enrollment == null)
        {
            return BadRequest(new { message = "Unable to create enrollment. Please verify student and course exist and course is not full." });
        }

        return CreatedAtAction(nameof(GetById), new { id = enrollment.Id }, enrollment);
    }

    /// <summary>
    /// Update an enrollment (e.g., grades, status)
    /// </summary>
    /// <param name="id">Enrollment ID</param>
    /// <param name="dto">Updated enrollment data</param>
    /// <returns>Updated enrollment</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Instructor")]
    [ProducesResponseType(typeof(EnrollmentReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EnrollmentReadDto>> Update(int id, [FromBody] EnrollmentUpdateDto dto)
    {
        var enrollment = await _enrollmentService.UpdateAsync(id, dto);
        
        if (enrollment == null)
        {
            return NotFound(new { message = $"Enrollment with ID {id} not found" });
        }

        return Ok(enrollment);
    }

    /// <summary>
    /// Delete an enrollment (unenroll)
    /// </summary>
    /// <param name="id">Enrollment ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Student")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _enrollmentService.DeleteAsync(id);
        
        if (!result)
        {
            return NotFound(new { message = $"Enrollment with ID {id} not found" });
        }

        return NoContent();
    }
}
