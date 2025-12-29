using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.DTOs;

public class CreateUserQuoteFavoriteDto
{
    [Required]
    public int QuoteId { get; set; }
}
