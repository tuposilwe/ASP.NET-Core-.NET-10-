using System;
using Gamestore.Api.Data;
using Gamestore.Api.Dtos;
using Gamestore.Api.Models;

namespace Gamestore.Api.Endppoints;

public static class GamesEndpoints
{
    const string GetNameEndpointName = "GetGame";
    private static readonly List<GameDto> games = [
     new (1, "Street Fighter II", "Fighting", 19.99M, new DateOnly(1992, 7, 15)),
    new (2, "The Legend of Zelda: Ocarina of Time", "Action-Adventure", 59.99M, new DateOnly(1998, 11, 21)),
    new (3, "Final Fantasy VII", "Role-Playing", 49.99M, new DateOnly(1997, 1, 31)),
    new (4, "Halo: Combat Evolved", "First-Person Shooter", 49.99M, new DateOnly(2001, 11, 15)),
    new (5, "Super Mario World", "Platformer", 29.99M, new DateOnly(1990, 11, 21)),
    new (6, "Minecraft", "Sandbox", 26.95M, new DateOnly(2011, 11, 18))
   ];

    public static void MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games");

        // GET /games
        group.MapGet("/", () => games);


        // GET /games/1
        group.MapGet("/{id}", (int id) =>
        {
            var game = games.Find(game => game.Id == id);

            return game is null ? Results.NotFound() : Results.Ok(game);
        })
          .WithName(GetNameEndpointName);

        // POST /games
        group.MapPost("/", (CreateGameDto newGame,GameStoreContext dbContext) =>
        {
            Game game = new()
            {
                Name = newGame.Name,
                GenreId=  newGame.GenreId,
                Price = newGame.Price,
                ReleaseDate= newGame.ReleaseDate
            };


            // GameDto game = new(
            //     games.Count + 1,
            //     newGame.Name,
            //     newGame.Genre,
            //     newGame.Price,
            //     newGame.ReleaseDate
            // );
            // games.Add(game);

            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            GameDetailsDto gameDto = new(
                game.Id,
                game.Name,
                game.GenreId,
                game.Price,
                game.ReleaseDate
            );

            return Results.CreatedAtRoute(
                GetNameEndpointName,
                new { id = gameDto.Id },
                gameDto);
        });

        // PUT /games/1
        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) =>
        {
            var index = games.FindIndex(game => game.Id == id);

            if (index == -1)
            {
                return Results.NotFound();
            }

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
        group.MapDelete("/{id}", (int id) =>
        {
            games.RemoveAll(game => game.Id == id);

            return Results.NoContent();
        });
    }


}
