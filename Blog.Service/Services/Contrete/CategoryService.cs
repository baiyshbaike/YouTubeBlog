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
}