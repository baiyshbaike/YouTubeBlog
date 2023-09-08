using AutoMapper;
using Blog.Data.UnitOfWorks;
using Blog.Entity.DTOs.Categories;
using Blog.Entity.Entities;
using Blog.Service.Services.Abstractioins;

namespace Blog.Service.Services.Contrete;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public CategoryService(IUnitOfWork unitOfWork,IMapper mapper)
    {
        this._mapper = mapper;
        this._unitOfWork = unitOfWork;
    }
    public async Task<List<CategoryDto>> GetAllCategoriesNonDeleted()
    {
        var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(x=>!x.IsDeleted);
        var map = _mapper.Map<List<CategoryDto>>(categories);
        return map;
    }
}