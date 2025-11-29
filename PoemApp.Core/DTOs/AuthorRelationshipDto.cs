using PoemApp.Core.Enums;

namespace PoemApp.Core.DTOs;

public class AuthorRelationshipDto
{
    public int Id { get; set; }
    public int FromAuthorId { get; set; }
    public string FromAuthorName { get; set; } = null!;
    public int ToAuthorId { get; set; }
    public string ToAuthorName { get; set; } = null!;
    public RelationshipTypeEnum RelationshipType { get; set; }
    public string RelationshipTypeDisplayName { get; set; } = null!;
}