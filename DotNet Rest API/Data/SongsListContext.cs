using DotNet_Rest_API.Entities;
using Microsoft.EntityFrameworkCore;
using System.CodeDom.Compiler;

namespace DotNet_Rest_API.Data
{
    public class SongsListContext(DbContextOptions<SongsListContext> options) : DbContext(options)
    {
        public DbSet<Song> Songs => Set<Song>();
        public DbSet<Genre> Genres => Set<Genre>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Genre>().HasData(
                new { Id = 1, Name = "Pop" },
                new { Id = 2, Name = "Rock" },
                new { Id = 3, Name = "Metal" },
                new { Id = 4, Name = "Indie" },
                new { Id = 5, Name = "Rap" }
            );
        }
    }
}
