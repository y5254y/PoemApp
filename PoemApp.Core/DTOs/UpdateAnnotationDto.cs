// UpdateAnnotationDto.cs
using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.DTOs
{
    public class UpdateAnnotationDto
    {
        [Required]
        [StringLength(400)]
        public string HighlightText { get; set; } = null!;

        [Required]
        [StringLength(600)]
        public string Comment { get; set; } = null!;

        [Required]
        public int StartIndex { get; set; }

        [Required]
        public int EndIndex { get; set; }
    }
}