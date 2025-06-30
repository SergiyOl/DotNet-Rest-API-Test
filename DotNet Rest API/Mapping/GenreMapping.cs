using DotNet_Rest_API.DTOs;
using DotNet_Rest_API.Entities;

namespace DotNet_Rest_API.Mapping
{
    public static class GenreMapping
    {
        public static GenreDto ToDto(this Genre genre)
        {
            return new(
                genre.Id,
                genre.Name
            );
        }
    }
}
