using PoemApp.Core.Enums;

namespace PoemApp.Core.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CategoryGroup? Group { get; set; }
    public int? ParentId { get; set; }
    public string? ParentName { get; set; }
    public int SortOrder { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsLeaf { get; set; }
    public List<PoemDto> Poems { get; set; } = new List<PoemDto>();
}