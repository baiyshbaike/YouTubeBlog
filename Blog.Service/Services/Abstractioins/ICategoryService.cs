using Blog.Entity.DTOs.Categories;

namespace Blog.Service.Services.Abstractioins;

public interface ICategoryService
{ 
    Task<List<CategoryDto>> GetAllCategoriesNonDeleted();
    Task CreateCategoryAsync(CategoryAddDto categoryAddDto);
}