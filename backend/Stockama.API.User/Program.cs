using Stockama.Utils.Extensions;
using Stockama.Helper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddEnvironmentVariables();
EnvironmentVariables.AuthClientType = "web";

builder.Services.AddCommonServices();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.AddCommonServices();
// app.UseHttpsRedirection();
app.UseGlobalErrorHandler();

app.UseCustomAuthenticationMiddleware();
app.UseAuthenticationMiddleware();

app.MapControllers();

app.Run();
