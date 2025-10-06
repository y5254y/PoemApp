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
    public string Dynasty { get; set; } = null!;

    public string? Biography { get; set; }

    // 作者关系（自引用）
    public ICollection<AuthorRelationship> Relationships { get; set; } = [];

    // 作品集合
    public ICollection<Poem> Poems { get; set; } = [];
}

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
    public string RelationshipType { get; set; } = null!;// 如"师徒"、"好友"、"父子"、"兄弟"、
}