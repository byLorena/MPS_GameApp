using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MvcGame.Data;
using System;
using System.Linq;

namespace MvcGame.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MvcGameContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MvcGameContext>>()))
            {
                // Look for any movies.
                if (context.Game.Any())
                {
                    return;   // DB has been seeded
                }

                context.Game.AddRange(
                    new Game
                    {
                        Title = "The Last of Us Part I",
                        ReleaseDate = DateTime.Parse("2013-6-14"),
                        Genre = "Adventure",
                        Price = 60
                    },

                    new Game
                    {
                        Title = "Beyond Two Souls ",
                        ReleaseDate = DateTime.Parse("2013-10-08"),
                        Genre = "Adventure",
                        Price = 20
                    },

                    new Game
                    {
                        Title = "Minecraft",
                        ReleaseDate = DateTime.Parse("2009-5-17"),
                        Genre = "Survival",
                        Price = 30
                    },

                    new Game
                    {
                        Title = "Dying Light",
                        ReleaseDate = DateTime.Parse("2015-1-26"),
                        Genre = "Survival Horror",
                        Price = 30
                    }
                );
                context.Review.Add(new Review { GameId = 1, PublishDate = DateTime.Now, ReviewMessage = "aaa", User = "test" });
                context.SaveChanges();
            }
        }
    }
}