using Gamestore.Api.Endppoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();

var app = builder.Build();

app.MapGamesEndpoints();

app.Run();
