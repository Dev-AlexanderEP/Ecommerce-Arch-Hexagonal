using Hangfire;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MixAndMatch API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = []
    });
}
else
{
    var hangfireLogin    = app.Configuration["Hangfire:Login"]!;
    var hangfirePassword = app.Configuration["Hangfire:Password"]!;
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = [new HangfireBasicAuthFilter(hangfireLogin, hangfirePassword)]
    });
}

HangfireJobScheduler.RegistrarJobsRecurrentes();

app.MapControllers();

app.Run();
