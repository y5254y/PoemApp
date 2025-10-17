using PoemApp.Core.Enums;

namespace PoemApp.Core.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public CategoryTypeEnum Type { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public List<PoemDto> Poems { get; set; } = new List<PoemDto>();
}