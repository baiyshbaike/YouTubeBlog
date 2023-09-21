using AutoMapper;
using Blog.Data.UnitOfWorks;
using Blog.Entity.DTOs.Articles;
using Blog.Entity.DTOs.Categories;
using Blog.Entity.Entities;
using Blog.Entity.Enums;
using Blog.Service.Extensions;
using Blog.Service.Services.Abstractioins;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Blog.Service.Services.Contrete;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ClaimsPrincipal _user;
    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        this._mapper = mapper;
        this._httpContextAccessor = httpContextAccessor;
        this._unitOfWork = unitOfWork;
        _user = httpContextAccessor.HttpContext.User;
    }
    public async Task<List<CategoryDto>> GetAllCategoriesNonDeleted()
    {
        var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(x => !x.IsDeleted);
        var map = _mapper.Map<List<CategoryDto>>(categories);
        return map;
    }
    public async Task CreateCategoryAsync(CategoryAddDto categoryAddDto)
    {
        var userEmail = _user.GetLoggedInEmail();
        Category category = new (categoryAddDto.Name, userEmail);
        await _unitOfWork.GetRepository<Category>().AddAsync(category);
        await _unitOfWork.SaveAsync();
    }
    public async Task<CategoryDto> GetCategoryNonDeletedAsync(Guid categoryId)
    {
        var category = await _unitOfWork.GetRepository<Category>()
            .GetAsync(x => !x.IsDeleted && x.Id == categoryId);
        var map = _mapper.Map<CategoryDto>(category);
        return map;
    }
    public async Task<string> UpdateCategoryeAsync(CategoryUpdateDto categoryUpdateDto)
    {
        var userEmail = _user.GetLoggedInEmail();
        var category = await _unitOfWork.GetRepository<Category>()
            .GetAsync(x => !x.IsDeleted && x.Id == categoryUpdateDto.Id);
        category.Name = categoryUpdateDto.Name;
        category.ModifiedBy = _user.GetLoggedInEmail();
        category.ModifiedDate = DateTime.Now;
        await _unitOfWork.GetRepository<Category>().UpdateAsunc(category);
        await _unitOfWork.SaveAsync();
        return category.Name;
    }
    public async Task<string> SafeDeleteCategoryAsync(Guid categoryId)
    {
        var userEmail = _user.GetLoggedInEmail();
        var category = await _unitOfWork.GetRepository<Category>().GetByGuidAsync(categoryId);
        category.IsDeleted = true;
        category.DeletedDate = DateTime.Now;
        category.DeletedBy = userEmail;
        await _unitOfWork.GetRepository<Category>().UpdateAsunc(category);
        await _unitOfWork.SaveAsync();
        return category.Name;
    }

    public async Task<List<CategoryDto>> GetAllCategoriesDeleted()
    {
        var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(x => x.IsDeleted);
        var map = _mapper.Map<List<CategoryDto>>(categories);
        return map;
    }

    public async Task<string> UndoDeleteCategoryAsync(Guid categoryId)
    {
        var category = await _unitOfWork.GetRepository<Category>().GetByGuidAsync(categoryId);
        category.IsDeleted = false;
        category.DeletedDate = null;
        category.DeletedBy = null;
        await _unitOfWork.GetRepository<Category>().UpdateAsunc(category);
        await _unitOfWork.SaveAsync();
        return category.Name;
    }
}