using DotNet_Rest_API.DTOs;
using DotNet_Rest_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNet_Rest_API.Mapping
{
    public static class SongMapping
    {
        public static Song ToEntity(this CreateSongDto song)
        {
            return new Song()
            {
                Name = song.Name,
                GenreId = song.GenreId,
                Lenght = song.Lenght,
                Listens = song.Listens
            };
        }

        public static SongDto ToDto(this Song song)
        {
            return new(
                 song.Id,
                 song.Name,
                 song.Genre!.Name,
                 song.Lenght,
                 song.Listens
            );
        }
    }
}
