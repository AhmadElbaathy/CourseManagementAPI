using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CourseManagementAPI.DTOs;
using CourseManagementAPI.Services;

namespace CourseManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize] // Requires authentication for all endpoints
public class InstructorsController : ControllerBase
{
    private readonly IInstructorService _instructorService;

    public InstructorsController(IInstructorService instructorService)
    {
        _instructorService = instructorService;
    }

    /// <summary>
    /// Get all instructors
    /// </summary>
    /// <returns>List of instructors</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InstructorReadDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<InstructorReadDto>>> GetAll()
    {
        var instructors = await _instructorService.GetAllAsync();
        return Ok(instructors);
    }

    /// <summary>
    /// Get an instructor by ID
    /// </summary>
    /// <param name="id">Instructor ID</param>
    /// <returns>Instructor details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(InstructorReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InstructorReadDto>> GetById(int id)
    {
        var instructor = await _instructorService.GetByIdAsync(id);
        
        if (instructor == null)
        {
            return NotFound(new { message = $"Instructor with ID {id} not found" });
        }

        return Ok(instructor);
    }

    /// <summary>
    /// Create a new instructor
    /// </summary>
    /// <param name="dto">Instructor data</param>
    /// <returns>Created instructor</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")] // Only Admin can create instructors
    [ProducesResponseType(typeof(InstructorReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InstructorReadDto>> Create([FromBody] InstructorCreateDto dto)
    {
        var instructor = await _instructorService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = instructor.Id }, instructor);
    }

    /// <summary>
    /// Update an instructor
    /// </summary>
    /// <param name="id">Instructor ID</param>
    /// <param name="dto">Updated instructor data</param>
    /// <returns>Updated instructor</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Instructor")] // Admin or Instructor can update
    [ProducesResponseType(typeof(InstructorReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InstructorReadDto>> Update(int id, [FromBody] InstructorUpdateDto dto)
    {
        var instructor = await _instructorService.UpdateAsync(id, dto);
        
        if (instructor == null)
        {
            return NotFound(new { message = $"Instructor with ID {id} not found" });
        }

        return Ok(instructor);
    }

    /// <summary>
    /// Delete an instructor
    /// </summary>
    /// <param name="id">Instructor ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] // Only Admin can delete
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _instructorService.DeleteAsync(id);
        
        if (!result)
        {
            return NotFound(new { message = $"Instructor with ID {id} not found" });
        }

        return NoContent();
    }

    /// <summary>
    /// Get instructor profile
    /// </summary>
    /// <param name="id">Instructor ID</param>
    /// <returns>Instructor profile</returns>
    [HttpGet("{id}/profile")]
    [ProducesResponseType(typeof(InstructorProfileReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InstructorProfileReadDto>> GetProfile(int id)
    {
        var profile = await _instructorService.GetProfileAsync(id);
        
        if (profile == null)
        {
            return NotFound(new { message = $"Profile for instructor with ID {id} not found" });
        }

        return Ok(profile);
    }

    /// <summary>
    /// Create or update instructor profile
    /// </summary>
    /// <param name="id">Instructor ID</param>
    /// <param name="dto">Profile data</param>
    /// <returns>Updated profile</returns>
    [HttpPut("{id}/profile")]
    [Authorize(Roles = "Admin,Instructor")]
    [ProducesResponseType(typeof(InstructorProfileReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InstructorProfileReadDto>> UpdateProfile(int id, [FromBody] InstructorProfileCreateDto dto)
    {
        // Verify instructor exists
        var instructor = await _instructorService.GetByIdAsync(id);
        if (instructor == null)
        {
            return NotFound(new { message = $"Instructor with ID {id} not found" });
        }

        dto.InstructorId = id;
        var profile = await _instructorService.CreateOrUpdateProfileAsync(id, dto);
        return Ok(profile);
    }
}
