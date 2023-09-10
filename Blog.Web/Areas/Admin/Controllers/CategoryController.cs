using AutoMapper;
using Blog.Entity.DTOs.Articles;
using Blog.Entity.DTOs.Categories;
using Blog.Entity.Entities;
using Blog.Service.Extensions;
using Blog.Service.Services.Abstractioins;
using Blog.Service.Services.Contrete;
using Blog.Web.ResultMessages;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.ComponentModel.DataAnnotations;

namespace Blog.Web.Areas.Admin.Controllers
{
    [Area("admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService categoryService;

        public IValidator<Category> _validator;
        public IMapper _mapper;
        public IToastNotification _toastNotification;

        public CategoryController(ICategoryService categoryService, IValidator<Category> validator, IMapper mapper,IToastNotification toastNotification)
        {
            this.categoryService = categoryService;
            _validator = validator;
            _mapper = mapper;
            _toastNotification = toastNotification;
        }
        public async  Task<IActionResult> Index()
        {
            var categories = await categoryService.GetAllCategoriesNonDeleted();
            return View(categories);
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(CategoryAddDto categoryAddDto)
        {
            var map = _mapper.Map<Category> (categoryAddDto);
            var result = await _validator.ValidateAsync(map);
            if(result.IsValid)
            {
                await categoryService.CreateCategoryAsync(categoryAddDto);
                _toastNotification.AddSuccessToastMessage(Messages.Category.Add(categoryAddDto.Name), new ToastrOptions() { Title = "successful!" });
                return RedirectToAction("Index", "Category", new { Area = "Admin" });
            }
            result.AddToModelState(this.ModelState);
            return View();
        }
    }
}
