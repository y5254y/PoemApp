// PointsRecordDto.cs
using PoemApp.Core.Enums;

namespace PoemApp.Core.DTOs;

public class PointsRecordDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public PointsSource Source { get; set; }
    public string SourceDisplayName { get; set; } = null!;
    public int Points { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? RelatedId { get; set; }
}