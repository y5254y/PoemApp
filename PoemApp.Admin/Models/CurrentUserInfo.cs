namespace PoemApp.Admin.Models;

// 新增：当前用户信息模型
public class CurrentUserInfo
{
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}