using Gamestore.Api.Data;
using Gamestore.Api.Endppoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();

builder.AddGameStoreDb();

var app = builder.Build();

app.MapGamesEndpoints();

app.MigrateDb();

app.Run();
