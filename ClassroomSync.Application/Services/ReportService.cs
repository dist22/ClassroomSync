using ClassroomSync.Application.Interfaces.Client;
using ClassroomSync.Application.Interfaces.Repositories;
using ClassroomSync.Application.Interfaces.Sevices;
using ClassroomSync.Domain.Entities;

namespace ClassroomSync.Application.Services;

public class ReportService(IUserRepository userRepository, IGoogleSheetClient googleSheetClient) : IReportService
{
    public async Task SyncMasterReportAsync(CancellationToken cancellationToken = default)
    {
        // 1. Отримуємо дані з БД (у репозиторії має бути .AsNoTracking())
        var users = await userRepository.GetAllUsersWithFullProgressAsync(cancellationToken);
        if (users == null || !users.Any()) return;

        // 2. Індексуємо дані в пам'яті для миттєвого доступу O(1)

        // Пре-парсинг коротких імен студентів (щоб не робити .Split в кожному рядку)
        var studentShortNames = users.ToDictionary(
            u => u.Id,
            u =>
            {
                var nameParts = (u.LastName ?? "Без_Імені").Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return nameParts.Length > 0 ? nameParts[0] : "Без_Імені";
            });

        // Індексуємо студентів по курсах: CourseId -> List<User>
        var studentsByCourse = users
            .SelectMany(u => u.UserCourses.Select(uc => new { uc.CourseId, User = u }))
            .ToLookup(x => x.CourseId, x => x.User);

        // Індексуємо роботи за складеним ключем: (UserId, AssignmentId) -> Submission
        var submissionMap = users
            .SelectMany(u => (u.Submissions ?? Enumerable.Empty<StudentSubmission>())
                .Select(s => new { Key = (u.Id, s.AssignmentId), Submission = s }))
            .DistinctBy(x => x.Key)
            .ToDictionary(x => x.Key, x => x.Submission);

        // Збираємо унікальні курси
        var allCourses = users
            .SelectMany(u => u.UserCourses)
            .Select(uc => uc.Course)
            .DistinctBy(c => c.Id)
            .ToList();

        var courseDataMap = new Dictionary<string, List<IList<object>>>();
        var now = DateTime.UtcNow;

        // 3. Формуємо матриці в пам'яті
        foreach (var course in allCourses)
        {
            var sheetName = CleanSheetName(course.Name);
            var sheetData = new List<IList<object>>();

            // Отримуємо студентів курсу та відразу їх сортуємо
            var enrolledStudents = studentsByCourse[course.Id]
                .OrderBy(u => u.LastName)
                .ToList();

            if (!enrolledStudents.Any()) continue;

            // Рядок 1: Назва предмету
            string courseUrl = course.Url ?? "";
            string safeCourseName = (course.Name ?? "Курс").Replace("\"", "\"\"");
            string courseHeader = string.IsNullOrEmpty(courseUrl)
                ? course.Name
                : $"=HYPERLINK(\"{courseUrl}\"; \"{safeCourseName}\")";
            sheetData.Add(new List<object> { courseHeader });

            // Рядок 2: Заголовки
            var headers = new List<object> { "Назва дз", "Дедлайн", "Посилання" };
            headers.AddRange(enrolledStudents.Select(s => studentShortNames[s.Id]));
            sheetData.Add(headers);

            // Рядки завдань (Сортування: спочатку ті що з дедлайном, потім за датою дедлайну, потім за назвою)
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
                    assignment.DueDate?.ToString("dd.MM") ?? "?",
                    assignmentLinkForm
                };

                bool isOverdue = assignment.DueDate.HasValue && assignment.DueDate.Value < now;
                string maxPointsStr = assignment.MaxPoints.HasValue ? assignment.MaxPoints.Value.ToString() : "?";

                // O(1) пошук здачі для кожного студента
                foreach (var student in enrolledStudents)
                {
                    submissionMap.TryGetValue((student.Id, assignment.Id), out var submission);

                    string displayValue;
                    if (submission != null && submission.AssignedGrade.HasValue)
                    {
                        displayValue = $"{submission.AssignedGrade.Value}/{maxPointsStr}";
                    }
                    else if (submission != null && submission.State == "TURNED_IN")
                    {
                        displayValue = "Здано";
                    }
                    else if (isOverdue)
                    {
                        displayValue = "Прострочено";
                    }
                    else
                    {
                        displayValue = "Не здано";
                    }

                    row.Add(displayValue);
                }
                sheetData.Add(row);
            }

            courseDataMap[sheetName] = sheetData;
        }

        // 4. Паралельне відправлення даних у Google Sheets (з лімітом у 3 одночасних запити)
        using var semaphore = new SemaphoreSlim(3);
        var tasks = courseDataMap.Select(async courseSheet =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                await googleSheetClient.UpdateMasterSheetByNameAsync(courseSheet.Key, courseSheet.Value, cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
    }

    private string CleanSheetName(string originalName)
    {
        if (string.IsNullOrWhiteSpace(originalName)) return "Unknown";
        var cleaned = originalName.Replace(":", " ").Replace("?", " ").Replace("*", " ").Replace("[", " ").Replace("]", " ");
        return cleaned.Length > 90 ? cleaned.Substring(0, 90) : cleaned;
    }
}