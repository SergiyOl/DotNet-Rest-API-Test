using DotNet_Rest_API.Entities;
using Microsoft.EntityFrameworkCore;
using System.CodeDom.Compiler;

namespace DotNet_Rest_API.Data
{
    public class SongsListContext(DbContextOptions<SongsListContext> options) : DbContext(options)
    {
        public DbSet<Song> Songs => Set<Song>();
        public DbSet<Genre> Genres => Set<Genre>();
    }
}
