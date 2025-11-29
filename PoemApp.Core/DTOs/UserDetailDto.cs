// UserDetailDto.cs
using PoemApp.Core.Enums;

namespace PoemApp.Core.DTOs;

public class UserDetailDto : UserDto
{
    public List<UserFavoriteDto> Favorites { get; set; } = new List<UserFavoriteDto>();
    public List<PointsRecordDto> PointsRecords { get; set; } = new List<PointsRecordDto>();
    public List<AudioDto> UploadedAudios { get; set; } = new List<AudioDto>();
    public List<AnnotationDto> Annotations { get; set; } = new List<AnnotationDto>();
}