using ClassroomSync.Application;
using ClassroomSync.Infrastructure;
using ClassroomSync.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbConnection(builder.Configuration);
builder.Services.AddGoogleClients();
builder.Services.AddSheetServices(builder.Configuration);
builder.Services.AddGoogleAuthentication(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddAutoMapper();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();