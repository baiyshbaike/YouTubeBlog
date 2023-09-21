using Blog.Entity.DTOs.Categories;

namespace Blog.Service.Services.Abstractioins;

public interface ICategoryService
{ 
    Task<List<CategoryDto>> GetAllCategoriesNonDeleted();
    Task<List<CategoryDto>> GetAllCategoriesDeleted();
    Task CreateCategoryAsync(CategoryAddDto categoryAddDto);
    Task<CategoryDto> GetCategoryNonDeletedAsync(Guid categoryId);
    Task<string> UpdateCategoryeAsync(CategoryUpdateDto categoryUpdateDto);
    Task<string> SafeDeleteCategoryAsync(Guid categoryId);
    Task<string> UndoDeleteCategoryAsync(Guid categoryId);
}