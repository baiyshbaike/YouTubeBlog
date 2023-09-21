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
        private readonly ICategoryService _categoryService;

        public IValidator<Category> _validator;
        public IMapper _mapper;
        public IToastNotification _toastNotification;

        public CategoryController(ICategoryService categoryService, IValidator<Category> validator, IMapper mapper,IToastNotification toastNotification)
        {
            _categoryService = categoryService;
            _validator = validator;
            _mapper = mapper;
            _toastNotification = toastNotification;
        }
        [HttpGet]
        public async  Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesNonDeleted();
            return View(categories);
        }
        [HttpGet]
        public async  Task<IActionResult> DeletedCategory()
        {
            var categories = await _categoryService.GetAllCategoriesDeleted();
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
                await _categoryService.CreateCategoryAsync(categoryAddDto);
                _toastNotification.AddSuccessToastMessage(Messages.Category.Add(categoryAddDto.Name), new ToastrOptions() { Title = "successful!" });
                return RedirectToAction("Index", "Category", new { Area = "Admin" });
            }
            result.AddToModelState(this.ModelState);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddWithAjax([FromBody] CategoryAddDto categoryAddDto)
        {
            var map = _mapper.Map<Category>(categoryAddDto);
            var result = await _validator.ValidateAsync(map);
            if (result.IsValid)
            {
                await _categoryService.CreateCategoryAsync(categoryAddDto);
                _toastNotification.AddSuccessToastMessage(Messages.Category.Add(categoryAddDto.Name), new ToastrOptions() { Title = "successful!" });
                return Json(Messages.Category.Add(categoryAddDto.Name));
            }
            else { 
            _toastNotification.AddErrorToastMessage(result.Errors.First().ErrorMessage, new ToastrOptions() { Title = "filed" });
            return Json(result.Errors.First().ErrorMessage);
        }
        }
        [HttpGet]
        public async Task<IActionResult> Update(Guid categoryId)
        {
            var category = await _categoryService.GetCategoryNonDeletedAsync(categoryId);
            var categoryUpdateDto = _mapper.Map<CategoryUpdateDto>(category);
            return View(categoryUpdateDto);
        }

        [HttpPost]
        public async Task<IActionResult> Update(CategoryUpdateDto categoryUpdateDto)
        {
            var map = _mapper.Map<Category>(categoryUpdateDto);
            var result = await _validator.ValidateAsync(map);
            if (result.IsValid)
            {
                var message = await _categoryService.UpdateCategoryeAsync(categoryUpdateDto);
                _toastNotification.AddSuccessToastMessage(Messages.Category.Udpate(message), new ToastrOptions() { Title = "successful!" });
                return RedirectToAction("Index", "Category", new { Area = "Admin" });
            }
            else
            {
                result.AddToModelState(ModelState);
            }
            return View(categoryUpdateDto);
        }

        public async Task<IActionResult> Delete(Guid categoryId)
        {
            var title = await _categoryService.SafeDeleteCategoryAsync(categoryId);
            _toastNotification.AddSuccessToastMessage(Messages.Article.Delete(title), new ToastrOptions() { Title = "seccessful!" });
            return RedirectToAction("Index", "Category", new { Area = "Admin" });
        }
        public async Task<IActionResult> UndoDelete(Guid categoryId)
        {
            var title = await _categoryService.UndoDeleteCategoryAsync(categoryId);
            _toastNotification.AddSuccessToastMessage(Messages.Article.UndoDelete(title), new ToastrOptions() { Title = "seccessful!" });
            return RedirectToAction("Index", "Category", new { Area = "Admin" });
        }
    }
}
