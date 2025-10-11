using PoemApp.Core.Enums;

namespace PoemApp.Core.DTOs;

public class AuthorDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DynastyEnum Dynasty { get; set; }
    public string DynastyDisplayName { get; set; } = null!;
    public string? Biography { get; set; }
    public List<AuthorRelationshipDto> Relationships { get; set; } = new List<AuthorRelationshipDto>();
    public List<PoemDto> Poems { get; set; } = new List<PoemDto>();
}