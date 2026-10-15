namespace ClassroomSync.Application.Interfaces.Client;

public interface IGoogleSheetClient
{
    Task WriteToCellAsync(string spreadsheetId, string range, string value);
}