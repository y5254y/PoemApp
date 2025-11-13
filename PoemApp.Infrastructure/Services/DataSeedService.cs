using Microsoft.EntityFrameworkCore;
using PoemApp.Infrastructure.Data;  // 使用 Infrastructure 的 Data 上下文
using PoemApp.Core.Entities;
using PoemApp.Core.Enums;
using PoemApp.Core.Interfaces;

namespace PoemApp.Infrastructure.Services;

public interface IDataSeedService
{
    Task SeedAdminUserAsync();
    Task SeedTestDataAsync();
}

public class DataSeedService : IDataSeedService
{
    private readonly AppDbContext _context;

    public DataSeedService(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedAdminUserAsync()
    {
        // 检查是否已存在管理员用户
        if (await _context.Users.AnyAsync(u => u.Username == "admin"))
        {
            return; // 已存在，不重复创建
        }

        // 创建管理员用户
        var adminUser = new User
        {
            Username = "admin",
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        };

        // 设置密码（默认密码：admin123）
        SetPassword(adminUser, "admin123");

        _context.Users.Add(adminUser);
        await _context.SaveChangesAsync();

        Console.WriteLine("管理员用户创建成功：admin / admin123");
    }


    public async Task SeedTestUserAsync()
    {
        // 检查是否已存在管理员用户
        if (await _context.Users.AnyAsync(u => u.Username == "test"))
        {
            return; // 已存在，不重复创建
        }

        // 创建管理员用户
        var testUser = new User
        {
            Username = "test",
            Role = UserRole.Normal,
            CreatedAt = DateTime.UtcNow
        };

        // 设置密码（默认密码：admin123）
        SetPassword(testUser, "test123");

        _context.Users.Add(testUser);
        await _context.SaveChangesAsync();

        Console.WriteLine("管理员用户创建成功：admin / admin123");
    }

    public async Task SeedTestDataAsync()
    {
        await SeedAdminUserAsync();
        await SeedTestUserAsync();

        // 可以添加其他测试数据，比如示例作者、诗文等
        await SeedSampleAuthorsAsync();
        await SeedSamplePoemsAsync();
        await SeedSampleCategoriesAsync();

        Console.WriteLine("测试数据种子完成");
    }

    private async Task SeedSampleAuthorsAsync()
    {
        if (await _context.Authors.AnyAsync())
            return;

        var authors = new List<Author>
        {
            new Author { Name = "李白", Dynasty = DynastyEnum.Tang, Biography = "唐代著名诗人" },
            new Author { Name = "杜甫", Dynasty = DynastyEnum.Tang, Biography = "唐代著名诗人" },
            new Author { Name = "苏轼", Dynasty = DynastyEnum.Song, Biography = "宋代文学家" }
        };

        _context.Authors.AddRange(authors);
        await _context.SaveChangesAsync();
    }

    private async Task SeedSamplePoemsAsync()
    {
        if (await _context.Poems.AnyAsync())
            return;

        var liBai = await _context.Authors.FirstAsync(a => a.Name == "李白");
        var duFu = await _context.Authors.FirstAsync(a => a.Name == "杜甫");

        var poems = new List<Poem>
        {
            new Poem
            {
                Title = "静夜思",
                Content = "床前明月光，疑是地上霜。举头望明月，低头思故乡。",
                AuthorId = liBai.Id,
                Background = "李白在扬州旅舍所作，表达思乡之情。"
            },
            new Poem
            {
                Title = "春望",
                Content = "国破山河在，城春草木深。感时花溅泪，恨别鸟惊心。",
                AuthorId = duFu.Id,
                Background = "安史之乱期间，杜甫目睹长安城一片萧条。"
            }
        };

        _context.Poems.AddRange(poems);
        await _context.SaveChangesAsync();
    }

    private async Task SeedSampleCategoriesAsync()
    {
        if (await _context.Categories.AnyAsync())
            return;

        var categories = new List<Category>
        {
            new Category { Name = "小学", Description = "小学分类" },
            new Category { Name = "中学", Description = "中学分类" },
            new Category { Name = "大学", Description = "大学分类" }
        };

        _context.Categories.AddRange(categories);
        await _context.SaveChangesAsync();
    }

    private void SetPassword(User user, string password)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512();
        user.PasswordSalt = hmac.Key;
        user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }
}