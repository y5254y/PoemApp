namespace PoemApp.Infrastructure.Settings
{
    public class MiniAppSettings
    {
        public string AppId { get; set; } = string.Empty;
        public string AppSecret { get; set; } = string.Empty;
        public string Code2SessionUrl { get; set; } = "https://api.weixin.qq.com/sns/jscode2session";
    }

    public class WeChatSettings
    {
        public MiniAppSettings MiniApp { get; set; } = new MiniAppSettings();
    }
}