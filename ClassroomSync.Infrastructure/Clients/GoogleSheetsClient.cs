using ClassroomSync.Application.Interfaces.Client;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;

namespace ClassroomSync.Infrastructure.Clients;

public class GoogleSheetsClient(SheetsService sheetsService, IConfiguration configuration, ClientSecrets clientSecrets)
    : IGoogleSheetClient
{
    private readonly string _masterSpreadsheetId = configuration["GoogleApi:MasterSpreadsheetId"] ??
                                                   throw new ArgumentNullException($"MasterSpreadsheetId is missing");

    private readonly string _applicationName = configuration["GoogleApi:ApplicationName"] ?? "ClassroomSync";
    
    private readonly string _serviceAccountKeyPath = configuration["GoogleApi:CredentialsPath"] 
                                                     ?? throw new ArgumentNullException($"CredentialsPath is missing");
    
    private SheetsService CreateService()
    {
        GoogleCredential credential;
        using (var stream = new FileStream(_serviceAccountKeyPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped(SheetsService.Scope.Spreadsheets);
        }

        return new SheetsService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = _applicationName
        });
    }

    public async Task UpdateMasterSheetByNameAsync(string sheetName, IList<IList<object>> data, CancellationToken cancellationToken = default)
    {
        var service = CreateService();

        var spreadsheet = await service.Spreadsheets.Get(_masterSpreadsheetId).ExecuteAsync(cancellationToken);
        var sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == sheetName);
        int sheetId;

        if (sheet == null)
        {
            var addRequest = new BatchUpdateSpreadsheetRequest { Requests = new List<Request> { new Request { AddSheet = new AddSheetRequest { Properties = new SheetProperties { Title = sheetName } } } } };
            var response = await service.Spreadsheets.BatchUpdate(addRequest, _masterSpreadsheetId).ExecuteAsync(cancellationToken);
            sheetId = response.Replies[0].AddSheet.Properties.SheetId ?? 0;
        }
        else
        {
            sheetId = sheet.Properties.SheetId ?? 0;
            await service.Spreadsheets.Values.Clear(new ClearValuesRequest(), _masterSpreadsheetId, $"{sheetName}").ExecuteAsync(cancellationToken);
        }

        // Вставляємо значення з підтримкою формул (USERENTERED)
        var updateRequest = service.Spreadsheets.Values.Update(new ValueRange { Values = data }, _masterSpreadsheetId, $"{sheetName}!A1");
        updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        await updateRequest.ExecuteAsync(cancellationToken);

        // Форматування
        int totalColumns = data.Count > 1 ? data[1].Count : 3;
        int frozenCols = Math.Min(3, totalColumns);

        var batchRequests = new List<Request>();

        // Скасовуємо попередні об'єднання клітинок
        batchRequests.Add(new Request { UnmergeCells = new UnmergeCellsRequest { Range = new GridRange { SheetId = sheetId } } });

        // Заморожуємо 2 верхні рядки та перші колонки
        batchRequests.Add(new Request {
            UpdateSheetProperties = new UpdateSheetPropertiesRequest {
                Properties = new SheetProperties { SheetId = sheetId, GridProperties = new GridProperties { FrozenRowCount = 2, FrozenColumnCount = frozenCols } },
                Fields = "gridProperties.frozenRowCount,gridProperties.frozenColumnCount"
            }
        });

        // Об'єднуємо 1-й рядок у межах закріплених колонок
        if (frozenCols > 1)
        {
            batchRequests.Add(new Request {
                MergeCells = new MergeCellsRequest {
                    Range = new GridRange { SheetId = sheetId, StartRowIndex = 0, EndRowIndex = 1, StartColumnIndex = 0, EndColumnIndex = frozenCols },
                    MergeType = "MERGE_ALL"
                }
            });
        }

        // Заголовок предмета (жирний шрифт, 14pt, по центру)
        batchRequests.Add(new Request {
            RepeatCell = new RepeatCellRequest {
                Range = new GridRange { SheetId = sheetId, StartRowIndex = 0, EndRowIndex = 1, StartColumnIndex = 0, EndColumnIndex = frozenCols },
                Cell = new CellData { UserEnteredFormat = new CellFormat { TextFormat = new TextFormat { Bold = true, FontSize = 14 }, HorizontalAlignment = "CENTER" } },
                Fields = "userEnteredFormat(textFormat,horizontalAlignment)"
            }
        });

        // Заголовки колонок (2-й рядок)
        batchRequests.Add(new Request {
            RepeatCell = new RepeatCellRequest {
                Range = new GridRange { SheetId = sheetId, StartRowIndex = 1, EndRowIndex = 2 },
                Cell = new CellData { UserEnteredFormat = new CellFormat { TextFormat = new TextFormat { Bold = true } } },
                Fields = "userEnteredFormat(textFormat)"
            }
        });

        // Видаляємо старі правила умовного форматування
        var freshSpreadsheet = await service.Spreadsheets.Get(_masterSpreadsheetId).ExecuteAsync(cancellationToken);
        int ruleCount = freshSpreadsheet.Sheets.First(s => s.Properties.SheetId == sheetId).ConditionalFormats?.Count ?? 0;
        for (int i = ruleCount - 1; i >= 0; i--)
            batchRequests.Add(new Request { DeleteConditionalFormatRule = new DeleteConditionalFormatRuleRequest { Index = i, SheetId = sheetId } });

        // Умовне форматування (кольори):
        // 1. Зелений для оцінок (містить "/")
        batchRequests.Add(CreateContainsColorRule(sheetId, "/", 0.7f, 0.95f, 0.7f)); 
        // 2. Жовтий для "Здано"
        batchRequests.Add(CreateExactColorRule(sheetId, "Здано", 1.0f, 0.95f, 0.6f)); 
        // 3. Червоний для "Прострочено"
        batchRequests.Add(CreateExactColorRule(sheetId, "Прострочено", 0.95f, 0.65f, 0.65f)); 
        // 4. Сірий для "Не здано"
        batchRequests.Add(CreateExactColorRule(sheetId, "Не здано", 0.9f, 0.9f, 0.9f));

        await service.Spreadsheets.BatchUpdate(new BatchUpdateSpreadsheetRequest { Requests = batchRequests }, _masterSpreadsheetId).ExecuteAsync(cancellationToken);
    }

    private Request CreateExactColorRule(int sheetId, string text, float r, float g, float b)
    {
        return new Request {
            AddConditionalFormatRule = new AddConditionalFormatRuleRequest {
                Rule = new ConditionalFormatRule {
                    Ranges = new List<GridRange> { new GridRange { SheetId = sheetId, StartRowIndex = 2, StartColumnIndex = 3 } },
                    BooleanRule = new BooleanRule {
                        Condition = new BooleanCondition { Type = "TEXT_EQ", Values = new List<ConditionValue> { new ConditionValue { UserEnteredValue = text } } },
                        Format = new CellFormat { BackgroundColor = new Color { Red = r, Green = g, Blue = b } }
                    }
                }, Index = 0
            }
        };
    }

    private Request CreateContainsColorRule(int sheetId, string text, float r, float g, float b)
    {
        return new Request {
            AddConditionalFormatRule = new AddConditionalFormatRuleRequest {
                Rule = new ConditionalFormatRule {
                    Ranges = new List<GridRange> { new GridRange { SheetId = sheetId, StartRowIndex = 2, StartColumnIndex = 3 } },
                    BooleanRule = new BooleanRule {
                        Condition = new BooleanCondition { Type = "TEXT_CONTAINS", Values = new List<ConditionValue> { new ConditionValue { UserEnteredValue = text } } },
                        Format = new CellFormat { BackgroundColor = new Color { Red = r, Green = g, Blue = b } }
                    }
                }, Index = 0
            }
        };
    }
}