using MixAndMatch.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MixAndMatch API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.MapControllers();

app.Run();
