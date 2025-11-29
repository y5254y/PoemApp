// CreateUserFavoriteDto.cs
using System.ComponentModel.DataAnnotations;

namespace PoemApp.Core.DTOs;

public class CreateUserFavoriteDto
{
    [Required]
    public int PoemId { get; set; }
}