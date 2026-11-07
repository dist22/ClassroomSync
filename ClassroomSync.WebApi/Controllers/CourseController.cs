using ClassroomSync.Application.DTOs.Responses;
using ClassroomSync.Application.Interfaces.Sevices;
using Microsoft.AspNetCore.Mvc;

namespace ClassroomSync.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourseController(IAssignmentService assignmentService, ICourseService courseService) : ControllerBase
{
    [HttpGet("{courseId:guid}/assignments/user/{userId:guid}")]
    public async Task<IActionResult> GetCourseAssignment(Guid courseId, Guid userId,
        CancellationToken cancellationToken)
    {
        var assignment = await assignmentService.GetAssignmentForCourseAsync(courseId, userId, cancellationToken);
        if (!assignment.Any())
        {
            return NotFound("Нічого не знайдено, гасися");
        }
        return Ok(assignment);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseResponseDto>>> GetUserCoursesAsync(Guid userId,
        CancellationToken cancellationToken)
    {
        var courses = await courseService.GetUserCoursesAsync(userId, cancellationToken);
        if (!courses.Any()) return NotFound("Курсів не знайдено!");
        return Ok(courses);
    }
}