using ClassroomSync.Application.Interfaces.Repositories;
using ClassroomSync.Domain.Entities;
using ClassroomSync.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ClassroomSync.Infrastructure.Repositories;

public class CourseRepository(AppDbContext dbContext) : ICourseRepository
{
    private readonly DbSet<Course> _courses = dbContext.Courses;
    
    public async Task<List<Course>> GetByGoogleIdsAsync(IEnumerable<string> googleCourseIds, CancellationToken cancellationToken) 
        => await _courses.Where(c => googleCourseIds.Contains(c.GoogleCourseId)).ToListAsync(cancellationToken);
    

    public async Task AddAsync(Course course, CancellationToken cancellationToken) 
        => await _courses.AddAsync(course, cancellationToken);
}