using DotNet_Rest_API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.CodeDom.Compiler;

namespace DotNet_Rest_API.Data
{
    public class AppDBContext : IdentityDbContext<AppUser>
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        {}

        public DbSet<Song> Songs { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Pop" },
                new Genre { Id = 2, Name = "Rock" },
                new Genre { Id = 3, Name = "Metal" },
                new Genre { Id = 4, Name = "Indie" },
                new Genre { Id = 5, Name = "Rap" }
            );
        }
    }
}

