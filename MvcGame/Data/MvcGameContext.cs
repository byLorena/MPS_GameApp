using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcGame.Models;

namespace MvcGame.Data
{
    public class MvcGameContext : DbContext
    {
        public MvcGameContext (DbContextOptions<MvcGameContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasMany<Review>().WithOne(a => a.Game).HasForeignKey(a => a.GameId);
            });
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasOne<Game>().WithMany(a => a.Reviews);
            });
        }
        public DbSet<MvcGame.Models.Game> Game { get; set; } = default!;
        public DbSet<MvcGame.Models.Review> Review { get; set; } = default!;
    }
}
