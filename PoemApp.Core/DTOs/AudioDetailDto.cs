// AudioDetailDto.cs
namespace PoemApp.Core.DTOs;

public class AudioDetailDto : AudioDto
{
    public List<AudioRatingDto> Ratings { get; set; } = new List<AudioRatingDto>();
}