// AnnotationDto.cs
namespace PoemApp.Core.DTOs
{
    public class AnnotationDto
    {
        public int Id { get; set; }
        public int PoemId { get; set; }
        public string PoemTitle { get; set; } = null!;
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string HighlightText { get; set; } = null!;
        public string Comment { get; set; } = null!;
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}