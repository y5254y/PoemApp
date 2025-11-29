using Microsoft.EntityFrameworkCore;
using PoemApp.Core.Entities;


namespace PoemApp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSet属性对应数据库表
    public DbSet<Poem> Poems { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<PoemCategory> PoemCategories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserFavorite> UserFavorites { get; set; }
    public DbSet<Annotation> Annotations { get; set; }
    public DbSet<Audio> Audios { get; set; }
    public DbSet<AudioRating> AudioRatings { get; set; }
    public DbSet<AuthorRelationship> AuthorRelationships { get; set; }
    public DbSet<PointsRecord> PointsRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置多对多关系
        modelBuilder.Entity<PoemCategory>()
            .HasKey(pc => new { pc.PoemId, pc.CategoryId });

        modelBuilder.Entity<PoemCategory>()
            .HasOne(pc => pc.Poem)
            .WithMany(p => p.Categories)
            .HasForeignKey(pc => pc.PoemId);

        modelBuilder.Entity<PoemCategory>()
            .HasOne(pc => pc.Category)
            .WithMany(c => c.Poems)
            .HasForeignKey(pc => pc.CategoryId);

        // 配置用户收藏关系
        modelBuilder.Entity<UserFavorite>()
            .HasKey(uf => new { uf.UserId, uf.PoemId });

        modelBuilder.Entity<UserFavorite>()
            .HasOne(uf => uf.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(uf => uf.UserId);

        modelBuilder.Entity<UserFavorite>()
            .HasOne(uf => uf.Poem)
            .WithMany(p => p.FavoritedBy)
            .HasForeignKey(uf => uf.PoemId);

        // 配置作者关系（自引用）
        modelBuilder.Entity<AuthorRelationship>()
            .HasKey(ar => ar.Id);

        modelBuilder.Entity<AuthorRelationship>()
            .HasOne(ar => ar.FromAuthor)
            .WithMany(a => a.Relationships)
            .HasForeignKey(ar => ar.FromAuthorId)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<AuthorRelationship>()
            .HasOne(ar => ar.ToAuthor)
            .WithMany()
            .HasForeignKey(ar => ar.ToAuthorId)
            .OnDelete(DeleteBehavior.ClientCascade);

        // 配置音频评分关系
        modelBuilder.Entity<AudioRating>()
            .HasKey(ar => ar.Id);

        modelBuilder.Entity<AudioRating>()
            .HasOne(ar => ar.Audio)
            .WithMany(a => a.Ratings)
            .HasForeignKey(ar => ar.AudioId);

        modelBuilder.Entity<AudioRating>()
            .HasOne(ar => ar.User)
            .WithMany(u => u.Ratings)
            .HasForeignKey(ar => ar.UserId);


        // 配置 PointsRecord 实体
        modelBuilder.Entity<PointsRecord>()
            .HasKey(pr => pr.Id);

        modelBuilder.Entity<PointsRecord>()
            .HasOne(pr => pr.User)
            .WithMany(u => u.PointsRecords)
            .HasForeignKey(pr => pr.UserId);

        // Category self-reference (Parent/Children) and indexes for lookup/uniqueness
        modelBuilder.Entity<Category>()
            .HasOne(c => c.Parent)
            .WithMany(p => p.Children)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index on Name for fast lookup
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .HasDatabaseName("IX_Categories_Name");

        // Composite unique index on (Group, Name) to prevent duplicates within the same group
        modelBuilder.Entity<Category>()
            .HasIndex(c => new { c.Group, c.Name })
            .IsUnique()
            .HasDatabaseName("UX_Categories_Group_Name");
    }
}
