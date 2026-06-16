using LifeAdvisor.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

app.UseApiMiddleware();

app.Run();
