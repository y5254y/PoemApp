namespace PoemApp.Core.DTOs
{
    public class WeChatCode2SessionResponseDto
    {
        public int? Errcode { get; set; }
        public string? Errmsg { get; set; }
        public string? OpenId { get; set; }
        public string? SessionKey { get; set; }
        public string? UnionId { get; set; }
    }
}