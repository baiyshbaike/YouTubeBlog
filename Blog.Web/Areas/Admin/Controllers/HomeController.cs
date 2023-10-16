using Blog.Entity.Entities;
using Blog.Service.Services.Abstractioins;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(IArticleService articleService, UserManager<AppUser> userManager)
        {
            this._articleService = articleService;
            this._userManager = userManager;
        }
        public async Task<ActionResult> Index()
        {
            var articles =  await _articleService.GetAllArticlesWithCategoryNonDeletedAsync();
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
            return View(articles);
        }
    }
}
