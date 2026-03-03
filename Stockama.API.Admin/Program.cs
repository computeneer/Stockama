using Ganss.Xss;
using Stockama.Core.Middlewares;
using Stockama.Utils.Extensions;
using Stockama.API.Admin.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddEnvironmentVariables();

builder.Services.AddCommonServices();
builder.Services.AddSingleton<IHtmlSanitizer>(_ => new HtmlSanitizer());

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

app.UseCustomAuthenticationMiddleware();
app.UseMiddleware<SuperAdminOnlyMiddleware>();
// app.UseAuthenticationMiddleware();

app.MapControllers();

app.Run();
