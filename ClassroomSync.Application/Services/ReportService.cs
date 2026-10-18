using ClassroomSync.Application.Interfaces.Client;
using ClassroomSync.Application.Interfaces.Repositories;
using ClassroomSync.Application.Interfaces.Sevices;
using ClassroomSync.Domain.Entities;

namespace ClassroomSync.Application.Services;

public class ReportService(IUserRepository userRepository, IGoogleSheetClient googleSheetClient) : IReportService
{
    public async Task SyncMasterReportAsync(CancellationToken cancellationToken = default)
    {
        var users = await userRepository.GetAllUsersWithFullProgressAsync(cancellationToken);
        
        var allCourses = new Dictionary<Guid, Course>(); 
        foreach (var user in users)
            foreach (var userCourse in user.UserCourses)
                allCourses.TryAdd(userCourse.CourseId, userCourse.Course);

        var courseDataMap = new Dictionary<string, List<IList<object>>>();

        foreach (var courseKvp in allCourses)
        {
            var course = courseKvp.Value;
            var sheetName = CleanSheetName(course.Name);
            var sheetData = new List<IList<object>>();

            var enrolledStudents = users
                .Where(u => u.UserCourses.Any(uc => uc.CourseId == course.Id))
                .OrderBy(u => u.LastName) 
                .ToList();

            // 1. РЯДОК 1: Назва предмету з гіперпосиланням 
            string courseUrl = course.Url ?? ""; 
            string safeCourseName = (course.Name ?? "Курс").Replace("\"", "\"\"");
            string courseHeader = string.IsNullOrEmpty(courseUrl) 
                ? course.Name 
                : $"=HYPERLINK(\"{courseUrl}\"; \"{safeCourseName}\")";
            sheetData.Add(new List<object> { courseHeader });

            // 2. РЯДОК 2: Заголовки (Назва дз, Дедлайн, Посилання + Імена студентів)
            var headers = new List<object> { "Назва дз", "Дедлайн", "Посилання" };
            headers.AddRange(enrolledStudents.Select(s => 
            {
                var nameParts = (s.LastName ?? "Без_Імені").Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return nameParts.Length > 0 ? nameParts[0] : "Без_Імені";
            }));
            sheetData.Add(headers);

            // 3. РЯДКИ ЗАВДАНЬ
            var now = DateTime.UtcNow;

            var sortedAssignments = course.Assignments
                .OrderBy(a => a.DueDate.HasValue ? 0 : 1)
                .ThenBy(a => a.DueDate)
                .ThenBy(a => a.Title);

            foreach (var assignment in sortedAssignments)
            {
                string assignmentUrl = assignment.Course.Url ?? "";
                string assignmentLinkForm = string.IsNullOrEmpty(assignmentUrl) 
                    ? "-" 
                    : $"=HYPERLINK(\"{assignmentUrl}\"; \"Посилання\")";

                var row = new List<object>
                {
                    assignment.Title,
                    assignment.DueDate?.ToString("dd.MM") ?? "---",
                    assignmentLinkForm
                };

                bool isOverdue = assignment.DueDate.HasValue && assignment.DueDate.Value < now;

                // Оцінки та статуси для кожного студента
                foreach (var student in enrolledStudents)
                {
                    var submission = student.Submissions?.FirstOrDefault(s => s.AssignmentId == assignment.Id);
                    
                    string displayValue;

                    if (submission != null && submission.AssignedGrade.HasValue)
                    {
                        // Оцінено: показуємо бали з максимуму (наприклад "8/10")
                        var maxPoints = assignment.MaxPoints.HasValue ? assignment.MaxPoints.Value.ToString() : "?";
                        displayValue = $"{submission.AssignedGrade.Value}/{maxPoints}";
                    }
                    else if (submission != null && submission.State == "TURNED_IN")
                    {
                        // Здано, але не оцінено
                        displayValue = "Здано";
                    }
                    else if (isOverdue)
                    {
                        // Прострочено дедлайн
                        displayValue = "Прострочено";
                    }
                    else
                    {
                        // Не здано (дедлайн ще попереду або його немає)
                        displayValue = "Не здано";
                    }

                    row.Add(displayValue);
                }
                sheetData.Add(row);
            }
            courseDataMap[sheetName] = sheetData;
        }

        foreach (var courseSheet in courseDataMap)
        {
            await googleSheetClient.UpdateMasterSheetByNameAsync(courseSheet.Key, courseSheet.Value, cancellationToken);
        }
    }

    private string CleanSheetName(string originalName)
    {
        if (string.IsNullOrWhiteSpace(originalName)) return "Unknown";
        var cleaned = originalName.Replace(":", " ").Replace("?", " ").Replace("*", " ").Replace("[", " ").Replace("]", " ");
        return cleaned.Length > 90 ? cleaned.Substring(0, 90) : cleaned;
    }
}