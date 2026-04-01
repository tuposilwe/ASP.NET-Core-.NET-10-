using System;
using Gamestore.Api.Data;
using Gamestore.Api.Dtos;
using Gamestore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.Api.Endppoints;

public static class GamesEndpoints
{
    const string GetNameEndpointName = "GetGame";
    private static readonly List<GameSummaryDto> games = [
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
        group.MapGet("/", async(GameStoreContext dbContext) => 
            await dbContext.Games
            .Include(game => game.Genre)
            .Select(game => new GameSummaryDto(
                game.Id,
                game.Name,
                game.Genre!.Name,
                game.Price,
                game.ReleaseDate
            ))
            .AsNoTracking()
            .ToListAsync()
        );


        // GET /games/1
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            // var game = games.Find(game => game.Id == id);
            var game = await dbContext.Games.FindAsync(id);

            return game is null ? Results.NotFound() : Results.Ok(
                new GameDetailsDto(
                    game.Id,
                    game.Name,
                    game.GenreId,
                    game.Price,
                    game.ReleaseDate
                )
            );
        })
          .WithName(GetNameEndpointName);

        // POST /games
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = new()
            {
                Name = newGame.Name,
                GenreId = newGame.GenreId,
                Price = newGame.Price,
                ReleaseDate = newGame.ReleaseDate
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
            await dbContext.SaveChangesAsync();

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
        group.MapPut("/{id}", async(int id, UpdateGameDto updatedGame,GameStoreContext dbContext) =>
        {
            // var index = games.FindIndex(game => game.Id == id);

            var existingGame = await dbContext.Games.FindAsync(id);

            if (existingGame is null)
            {
                return Results.NotFound();
            }

            existingGame.Name = updatedGame.Name;
            existingGame.GenreId = updatedGame.GenreId;
            existingGame.Price = updatedGame.Price;
            existingGame.ReleaseDate = updatedGame.ReleaseDate;

            await dbContext.SaveChangesAsync();

            // games[index] = new GameSummaryDto(
            //     id,
            //     updatedGame.Name,
            //     updatedGame.Genre,
            //     updatedGame.Price,
            //     updatedGame.ReleaseDate
            // );

            return Results.NoContent();
        });

        // DELETE /games/1
        group.MapDelete("/{id}", async(int id,GameStoreContext dbContext) =>
        {
            // games.RemoveAll(game => game.Id == id);

            await dbContext.Games
               .Where(games => games.Id == id)
               .ExecuteDeleteAsync();

            return Results.NoContent();
        });
    }


}
