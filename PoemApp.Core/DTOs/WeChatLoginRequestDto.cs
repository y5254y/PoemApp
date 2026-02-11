namespace PoemApp.Core.DTOs
{
    public class WeChatLoginRequestDto
    {
        public string Code { get; set; } = string.Empty;
        // Optional encrypted data and iv if client wants server to decrypt user info
        public string? EncryptedData { get; set; }
        public string? Iv { get; set; }
    }
}