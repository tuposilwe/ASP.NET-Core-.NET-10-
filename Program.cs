using Gamestore.Api.Dtos;

const string GetNameEndpointName = "GetGame";

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

List<GameDto> games = [
    new (1, "Street Fighter II", "Fighting", 19.99M, new DateOnly(1992, 7, 15)),
    new (2, "The Legend of Zelda: Ocarina of Time", "Action-Adventure", 59.99M, new DateOnly(1998, 11, 21)),
    new (3, "Final Fantasy VII", "Role-Playing", 49.99M, new DateOnly(1997, 1, 31)),
    new (4, "Halo: Combat Evolved", "First-Person Shooter", 49.99M, new DateOnly(2001, 11, 15)),
    new (5, "Super Mario World", "Platformer", 29.99M, new DateOnly(1990, 11, 21)),
    new (6, "Minecraft", "Sandbox", 26.95M, new DateOnly(2011, 11, 18))
];


// GET /games
app.MapGet("/games", () => games);


// GET /games/1
app.MapGet("/games/{id}",(int id) => games.Find(game => game.Id == id))
  .WithName(GetNameEndpointName);

// POST /games
app.MapPost("/games",(CreateGameDto newGame)=>
{
    GameDto game = new(
        games.Count + 1,
        newGame.Name,
        newGame.Genre,
        newGame.Price,
        newGame.ReleaseDate
    );

    games.Add(game);

    return Results.CreatedAtRoute(
        GetNameEndpointName,
        new {id = game.Id},
        game);
});

// PUT /games/1
app.MapPut("/games/{id}",(int id,UpdateGameDto updatedGame) =>
{
    var index = games.FindIndex(game => game.Id == id);

    games[index] = new GameDto(
        id,
        updatedGame.Name,
        updatedGame.Genre,
        updatedGame.Price,
        updatedGame.ReleaseDate
    );

    return Results.NoContent();
});

// DELETE /games/1
app.MapDelete("/games/{id}",(int id)=>
{
    games.RemoveAll(game => game.Id == id);

    return Results.NoContent();
});

app.Run();
