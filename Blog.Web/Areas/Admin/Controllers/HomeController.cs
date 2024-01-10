using Blog.Entity.Entities;
using Blog.Service.Services.Abstractioins;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IDashboardService dashboardService;

        public HomeController(IArticleService articleService,IDashboardService dashboardService)
        {
            this._articleService = articleService;
            this.dashboardService = dashboardService;
        }
        public async Task<ActionResult> Index()
        {
            var articles =  await _articleService.GetAllArticlesWithCategoryNonDeletedAsync();
            return View(articles);
        }
        [HttpGet]
        public async Task<ActionResult> YearlyArticleCounts()
        {
            var count = await dashboardService.GetYearlyArtileCount();
            return Json(JsonConvert.SerializeObject(count));
        }
        [HttpGet]
        public async Task<ActionResult> TotalArticleCount()
        {
            var count = await dashboardService.GetTotalArticleCount();
            return Json(count);
        }
        [HttpGet]
        public async Task<ActionResult> TotalCategoryCount()
        {
            var count = await dashboardService.GetTotalCategoryCount();
            return Json(count);
        }
    }
}
