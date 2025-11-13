namespace PoemApp.Core.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Group { get; set; }
    public int? ParentId { get; set; }
    public List<PoemDto> Poems { get; set; } = new List<PoemDto>();
}