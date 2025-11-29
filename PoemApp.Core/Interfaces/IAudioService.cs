// IAudioService.cs
using PoemApp.Core.DTOs;

namespace PoemApp.Core.Interfaces;

public interface IAudioService
{
    Task<IEnumerable<AudioDto>> GetAllAudiosAsync();
    Task<AudioDetailDto> GetAudioByIdAsync(int id);
    Task<IEnumerable<AudioDto>> GetAudiosByPoemIdAsync(int poemId);
    Task<IEnumerable<AudioDto>> GetAudiosByUploaderIdAsync(int uploaderId);
    Task<AudioDto> AddAudioAsync(CreateAudioDto audioDto);
    Task UpdateAudioAsync(int id, UpdateAudioDto audioDto);
    Task DeleteAudioAsync(int id);

    // 评分相关方法
    Task<AudioRatingDto> AddAudioRatingAsync(int audioId, CreateAudioRatingDto ratingDto, int userId);
    Task UpdateAudioRatingAsync(int ratingId, UpdateAudioRatingDto ratingDto, int userId);
    Task DeleteAudioRatingAsync(int ratingId, int userId);
    Task<IEnumerable<AudioRatingDto>> GetAudioRatingsAsync(int audioId);
    Task<AudioRatingDto?> GetUserAudioRatingAsync(int audioId, int userId);
}