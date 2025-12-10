using PoemApp.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PoemApp.Core.Entities;

public class Author
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public DynastyEnum Dynasty { get; set; }

    public string? Biography { get; set; }

    // 作者关系（自引用）
    public ICollection<AuthorRelationship> Relationships { get; set; } = [];

    // 作品集合
    public ICollection<Poem> Poems { get; set; } = [];

    // 名言名句集合
    public ICollection<Quote> Quotes { get; set; } = [];
}


//带额外信息的自引用关系实体，例如师徒关系、亲属关系等。有自己的id做为主键
public class AuthorRelationship
{
    public int Id { get; set; }

    [Required]
    public int FromAuthorId { get; set; }
    public Author FromAuthor { get; set; } = null!;

    [Required]
    public int ToAuthorId { get; set; }
    public Author ToAuthor { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public RelationshipTypeEnum RelationshipType { get; set; }
}