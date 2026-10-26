namespace ClassroomSync.Application.Interfaces.Jobs;

public interface IJobDispatcher
{
    void EnqueueClassroomSync(Guid userId);

    void ScheduleSheetUpdate();
}