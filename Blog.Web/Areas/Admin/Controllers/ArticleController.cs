using AutoMapper;
using Blog.Entity.DTOs.Articles;
using Blog.Entity.Entities;
using Blog.Service.Extensions;
using Blog.Service.Services.Abstractioins;
using Blog.Web.ResultMessages;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace Blog.Web.Areas.Admin.Controllers;

[Area("admin")]
public class ArticleController : Controller
{
    private readonly IArticleService _articleService;
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;
    private readonly IValidator<Article> _validator;
    private readonly IToastNotification _toastNotification;

    public ArticleController(IArticleService articleService, ICategoryService categoryService,IMapper mapper,IValidator<Article> validator,IToastNotification toastNotification)
    {
        _categoryService = categoryService;
        _articleService = articleService;
        _mapper = mapper;
        _validator = validator;
        _toastNotification = toastNotification;
    }
    [HttpGet]
    [Authorize(Roles ="superadmin,admin, user")]
    public async Task<IActionResult> Index()
    {
        var articles = await _articleService.GetAllArticlesWithCategoryNonDeletedAsync();
        return View(articles);
    }
    [HttpGet]
    [Authorize(Roles = "superadmin,admin")]
    public async Task<IActionResult> DeletedArticle()
    {
        var articles = await _articleService.GetAllArticlesWithCategoryDeletedAsync();
        return View(articles);
    }
    [HttpGet]
    [Authorize(Roles = "superadmin,admin")]
    public async Task<IActionResult> Add()
    {
        var categories = await _categoryService.GetAllCategoriesNonDeleted();
        return View(new ArticleAddDto{Categories=categories});
    }
    [HttpPost]
    [Authorize(Roles = "superadmin,admin")]
    public async Task<IActionResult> Add(ArticleAddDto articleAddDto)
    {
        var map = _mapper.Map<Article>(articleAddDto);
        var result =await _validator.ValidateAsync(map);
        if (result.IsValid)
        {
           await _articleService.CreateArticleAsync(articleAddDto);
           // _toastNotification.AddSuccessToastMessage();
           // _toastNotification.AddSuccessToastMessage("Operation successful",new ToastrOptions(){Title = "successful!"});
           _toastNotification.AddSuccessToastMessage(Messages.Article.Add(articleAddDto.Title),new ToastrOptions(){Title = "successful!"});
           return RedirectToAction("Index", "Article", new { Area = "Admin" });
        }
        else
        {
            result.AddToModelState(this.ModelState);
        }
        var categories = await _categoryService.GetAllCategoriesNonDeleted();
        return View(new ArticleAddDto{Categories=categories});
    }

    [HttpGet]
    [Authorize(Roles = "superadmin,admin")]
    public async Task<IActionResult> Update(Guid articleId)
    {
        var article = await _articleService.GetArticleWIthCategoryNonDeletedAsync(articleId);
        var categories = await _categoryService.GetAllCategoriesNonDeleted();
        var articleUpdateDto = _mapper.Map<ArticleUpdateDto>(article);
        articleUpdateDto.Categories = categories;
        return View(articleUpdateDto);
    }

    [HttpPost]
    [Authorize(Roles = "superadmin,admin")]
    public async Task<IActionResult> Update(ArticleUpdateDto articleUpdateDto)
    {
        var map = _mapper.Map<Article>(articleUpdateDto);
        var result =await _validator.ValidateAsync(map);
        if (result.IsValid)
        {
            var message = await _articleService.UpdateArticleAsync(articleUpdateDto);
            _toastNotification.AddSuccessToastMessage(Messages.Article.Udpate(message),new ToastrOptions(){Title = "successful!"});
            return RedirectToAction("Index", "Article", new { Area = "Admin" });
        }
        else
        {
            result.AddToModelState(ModelState);
        }
        var categories = await _categoryService.GetAllCategoriesNonDeleted();
        articleUpdateDto.Categories = categories;
        return View(articleUpdateDto);
    }
    [Authorize(Roles = "superadmin,admin")]
    public async Task<IActionResult> Delete(Guid articleId)
    {
        var title = await _articleService.SafeDeleteArticleAsync(articleId);
        _toastNotification.AddSuccessToastMessage(Messages.Article.Delete(title), new ToastrOptions(){Title = "seccessful!"});
        return RedirectToAction("Index", "Article", new { Area = "Admin" });
    }
    [Authorize(Roles = "superadmin,admin")]     
    public async Task<IActionResult> UndoDelete(Guid articleId)
    {
        var title = await _articleService.UndoDeleteArticleAsync(articleId);
        _toastNotification.AddSuccessToastMessage(Messages.Article.UndoDelete(title), new ToastrOptions() { Title = "seccessful!" });
        return RedirectToAction("Index", "Article", new { Area = "Admin" });
    }
}