namespace ClassroomSync.Application.Models.Webhooks;

public class ClassroomNotification
{
    public string Collection { get; set; } 
    public string EventType { get; set; }  
    public ResourceIdData ResourceId { get; set; }
}