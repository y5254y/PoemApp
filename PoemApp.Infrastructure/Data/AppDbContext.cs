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
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserFavorite> UserFavorites { get; set; } = null!;
    public DbSet<UserQuoteFavorite> UserQuoteFavorites { get; set; } = null!;
    public DbSet<Annotation> Annotations { get; set; }
    public DbSet<Audio> Audios { get; set; }
    public DbSet<AudioRating> AudioRatings { get; set; }
    public DbSet<AuthorRelationship> AuthorRelationships { get; set; }
    public DbSet<PointsRecord> PointsRecords { get; set; }
    public DbSet<Quote> Quotes { get; set; }
    public DbSet<UserRecitation> UserRecitations { get; set; }
    public DbSet<RecitationReview> RecitationReviews { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<UserAchievement> UserAchievements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ensure EF maps table names to lower-case to match MySQL container naming (Linux MySQL is case-sensitive)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // set table name to lower-case
            var currentName = entityType.GetTableName();
            if (!string.IsNullOrEmpty(currentName))
            {
                entityType.SetTableName(currentName.ToLowerInvariant());
            }
        }

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

        // 配置用户对名句的收藏关系
        modelBuilder.Entity<UserQuoteFavorite>()
            .HasKey(uqf => new { uqf.UserId, uqf.QuoteId });

        modelBuilder.Entity<UserQuoteFavorite>()
            .HasOne(uqf => uqf.User)
            .WithMany(u => u.QuoteFavorites)
            .HasForeignKey(uqf => uqf.UserId);

        modelBuilder.Entity<UserQuoteFavorite>()
            .HasOne(uqf => uqf.Quote)
            .WithMany()
            .HasForeignKey(uqf => uqf.QuoteId);

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

        // Quote relationships
        modelBuilder.Entity<Quote>()
            .HasKey(q => q.Id);

        modelBuilder.Entity<Quote>()
            .HasOne(q => q.Author)
            .WithMany(a => a.Quotes)
            .HasForeignKey(q => q.AuthorId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Quote>()
            .HasOne(q => q.Poem)
            .WithMany()
            .HasForeignKey(q => q.PoemId)
            .OnDelete(DeleteBehavior.SetNull);

        // 配置用户背诵记录关系
        modelBuilder.Entity<UserRecitation>()
            .HasKey(ur => ur.Id);

        modelBuilder.Entity<UserRecitation>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.Recitations)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRecitation>()
            .HasOne(ur => ur.Poem)
            .WithMany()
            .HasForeignKey(ur => ur.PoemId);

        // 创建复合唯一索引，确保同一用户对同一首诗只能有一条背诵记录
        modelBuilder.Entity<UserRecitation>()
            .HasIndex(ur => new { ur.UserId, ur.PoemId })
            .IsUnique()
            .HasDatabaseName("UX_UserRecitation_User_Poem");

        // 配置复习记录关系
        modelBuilder.Entity<RecitationReview>()
            .HasKey(rr => rr.Id);

        modelBuilder.Entity<RecitationReview>()
            .HasOne(rr => rr.UserRecitation)
            .WithMany(ur => ur.Reviews)
            .HasForeignKey(rr => rr.UserRecitationId);

        // 配置用户成就关系
        modelBuilder.Entity<UserAchievement>()
            .HasKey(ua => ua.Id);

        modelBuilder.Entity<UserAchievement>()
            .HasOne(ua => ua.User)
            .WithMany(u => u.Achievements)
            .HasForeignKey(ua => ua.UserId);

        modelBuilder.Entity<UserAchievement>()
            .HasOne(ua => ua.Achievement)
            .WithMany(a => a.UserAchievements)
            .HasForeignKey(ua => ua.AchievementId);

        // 创建复合唯一索引，确保用户不能重复获得同一成就
        modelBuilder.Entity<UserAchievement>()
            .HasIndex(ua => new { ua.UserId, ua.AchievementId })
            .IsUnique()
            .HasDatabaseName("UX_UserAchievement_User_Achievement");
    }
}
