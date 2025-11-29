using System.ComponentModel.DataAnnotations;
using PoemApp.Core.Enums;

namespace PoemApp.Core.DTOs;

public class CreateAuthorRelationshipDto
{
    [Required]
    public int FromAuthorId { get; set; }

    [Required]
    public int ToAuthorId { get; set; }

    [Required]
    public RelationshipTypeEnum RelationshipType { get; set; }
}