using ClassroomSync.Application.Interfaces.Client;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace ClassroomSync.Infrastructure.Clients;

public class GoogleSheetsClient(SheetsService sheetsService) : IGoogleSheetClient
{
    public async Task WriteToCellAsync(string spreadsheetId, string range, string value)
    {
        var values = new List<IList<object>> { new List<object> { value } };
        var valueRange = new ValueRange { Values = values };

        var updateRequest = sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
        updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        
        await updateRequest.ExecuteAsync();
    }
}