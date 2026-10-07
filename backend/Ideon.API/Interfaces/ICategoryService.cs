using Ideon.API.DTOs.Categories;

namespace Ideon.API.Services.Interfaces;
// <summary>
// Defines the contract for category-related operations in the application.
public interface ICategoryService
{
    Task<IEnumerable<CategoryResponseDto>> GetAllAsync();

    Task<CategoryResponseDto?> GetByIdAsync(int id);
}