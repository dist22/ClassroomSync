using ClassroomSync.Application;
using ClassroomSync.Infrastructure;
using ClassroomSync.Infrastructure.Data;
using Hangfire;

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
builder.Services.AddHangFire(builder.Configuration);
builder.Services.AddWorkflowJob();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHangfireDashboard();
app.MapControllers();
app.Run();