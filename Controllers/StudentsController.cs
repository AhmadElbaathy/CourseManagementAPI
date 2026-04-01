using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CourseManagementAPI.DTOs;
using CourseManagementAPI.Services;

namespace CourseManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>
    /// Get all students
    /// </summary>
    /// <returns>List of students</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,Instructor")]
    [ProducesResponseType(typeof(IEnumerable<StudentReadDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StudentReadDto>>> GetAll()
    {
        var students = await _studentService.GetAllAsync();
        return Ok(students);
    }

    /// <summary>
    /// Get a student by ID
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <returns>Student details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StudentReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentReadDto>> GetById(int id)
    {
        var student = await _studentService.GetByIdAsync(id);
        
        if (student == null)
        {
            return NotFound(new { message = $"Student with ID {id} not found" });
        }

        return Ok(student);
    }

    /// <summary>
    /// Create a new student
    /// </summary>
    /// <param name="dto">Student data</param>
    /// <returns>Created student</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(StudentReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StudentReadDto>> Create([FromBody] StudentCreateDto dto)
    {
        var student = await _studentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
    }

    /// <summary>
    /// Update a student
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <param name="dto">Updated student data</param>
    /// <returns>Updated student</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Student")]
    [ProducesResponseType(typeof(StudentReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentReadDto>> Update(int id, [FromBody] StudentUpdateDto dto)
    {
        var student = await _studentService.UpdateAsync(id, dto);
        
        if (student == null)
        {
            return NotFound(new { message = $"Student with ID {id} not found" });
        }

        return Ok(student);
    }

    /// <summary>
    /// Delete a student
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _studentService.DeleteAsync(id);
        
        if (!result)
        {
            return NotFound(new { message = $"Student with ID {id} not found" });
        }

        return NoContent();
    }

    /// <summary>
    /// Get student enrollments
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <returns>List of enrollments</returns>
    [HttpGet("{id}/enrollments")]
    [ProducesResponseType(typeof(IEnumerable<EnrollmentSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<EnrollmentSummaryDto>>> GetEnrollments(int id)
    {
        // Verify student exists
        var student = await _studentService.GetByIdAsync(id);
        if (student == null)
        {
            return NotFound(new { message = $"Student with ID {id} not found" });
        }

        var enrollments = await _studentService.GetEnrollmentsAsync(id);
        return Ok(enrollments);
    }
}
