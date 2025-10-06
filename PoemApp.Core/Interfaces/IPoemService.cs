﻿using PoemApp.Core.DTOs;

namespace PoemApp.Core.Interfaces;

public interface IPoemService
{
    Task<IEnumerable<PoemDto>> GetAllPoemsAsync();
    Task<PoemDto> GetPoemByIdAsync(int id);
    Task<IEnumerable<PoemDto>> GetPoemsByCategoryAsync(string categoryName);
    Task<IEnumerable<PoemDto>> GetPoemsByAuthorAsync(int authorId);
    Task<PoemDto> AddPoemAsync(CreatePoemDto poemDto);
    Task UpdatePoemAsync(int id, UpdatePoemDto poemDto);
    Task DeletePoemAsync(int id);
}