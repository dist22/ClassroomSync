namespace ClassroomSync.Application.Interfaces.Client;

public interface IGoogleSheetClient
{
    public Task UpdateMasterSheetByNameAsync(string sheetName, IList<IList<object>> data, CancellationToken cancellationToken);
}