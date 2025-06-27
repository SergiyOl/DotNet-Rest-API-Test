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
        public static Song ToEntity(this UpdateSongDto song, int id)
        {
            return new Song()
            {
                Id = id,
                Name = song.Name,
                GenreId = song.GenreId,
                Lenght = song.Lenght,
                Listens = song.Listens
            };
        }

        public static SongSummaryDto ToSongSummaryDto(this Song song)
        {
            return new(
                 song.Id,
                 song.Name,
                 song.Genre!.Name,
                 song.Lenght,
                 song.Listens
            );
        }

        public static SongDetailsDto ToSongDetailsDto(this Song song)
        {
            return new(
                 song.Id,
                 song.Name,
                 song.GenreId,
                 song.Lenght,
                 song.Listens
            );
        }
    }
}
