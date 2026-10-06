using Ideon.API.DTOs.Ideas;

namespace Ideon.API.Services.Interfaces;
// <summary>
// Defines the contract for the idea service, which provides methods for managing ideas.
public interface IIdeaService
{   
    //Task - IO peration that returns a collection of IdeaResponseDto objects asynchronously. It retrieves all ideas from the data source and returns them as a list of IdeaResponseDto objects.
    Task<IEnumerable<IdeaResponseDto>> GetAllAsync();

    Task<IdeaResponseDto?> GetByIdAsync(int id);

    Task<IdeaResponseDto> CreateAsync(CreateIdeaDto dto);

    Task<IdeaResponseDto?> UpdateAsync(int id, UpdateIdeaDto dto);

    Task<bool> DeleteAsync(int id);
}