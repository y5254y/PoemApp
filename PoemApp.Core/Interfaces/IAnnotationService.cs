// IAnnotationService.cs
using PoemApp.Core.DTOs;

namespace PoemApp.Core.Interfaces
{
    public interface IAnnotationService
    {
        Task<IEnumerable<AnnotationDto>> GetAllAnnotationsAsync();
        Task<AnnotationDto> GetAnnotationByIdAsync(int id);
        Task<IEnumerable<AnnotationDto>> GetAnnotationsByPoemIdAsync(int poemId);
        Task<IEnumerable<AnnotationDto>> GetAnnotationsByUserIdAsync(int userId);
        Task<AnnotationDto> AddAnnotationAsync(CreateAnnotationDto annotationDto, int userId);
        Task UpdateAnnotationAsync(int id, UpdateAnnotationDto annotationDto, int userId);
        Task DeleteAnnotationAsync(int id, int userId);
    }
}