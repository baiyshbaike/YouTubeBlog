using AutoMapper;
using Blog.Data.UnitOfWorks;
using Blog.Entity.DTOs.Articles;
using Blog.Entity.Entities;
using Blog.Service.Services.Abstractioins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Blog.Service.Extensions;
using Blog.Service.Helpers.Images;
using Microsoft.AspNetCore.Http;
using Blog.Entity.Enums;

namespace Blog.Service.Services.Contrete
{
    public class ArticleService : IArticleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;
        private readonly IImageHelper _imageHelper;

        public ArticleService(IUnitOfWork unitOfWork,IMapper mapper,IHttpContextAccessor httpContextAccessor, IImageHelper imageHelper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._httpContextAccessor = httpContextAccessor;
            _imageHelper = imageHelper;
            _user = _httpContextAccessor.HttpContext.User;
        }
        public async Task<List<ArticleDto>>GetAllArticlesWithCategoryNonDeletedAsync()
        {
            var articles=  await _unitOfWork.GetRepository<Article>().GetAllAsync(x=>!x.IsDeleted,x=>x.Category, i => i.Image);
            var map = _mapper.Map<List<ArticleDto>>(articles);
            return map;
        }

        public async Task CreateArticleAsync(ArticleAddDto articleAddDto)
        {
            // var UserId = Guid.Parse("1CC65D1E-6154-450F-83F2-76609D2AC6ED");
            var userId = _user.GetLoggedInUserId();
            var userEmail = _user.GetLoggedInEmail();
            var imageUpload = await _imageHelper.Upload(articleAddDto.Title, articleAddDto.Photo, ImageType.Post);
            Image image = new(imageUpload.FullName, articleAddDto.Photo.ContentType, userEmail);
            await _unitOfWork.GetRepository<Image>().AddAsync(image);
            var article = new Article(articleAddDto.Title, articleAddDto.Content, userId, articleAddDto.CategoryId,
                image.Id,userEmail);
            await _unitOfWork.GetRepository<Article>().AddAsync(article);
            await _unitOfWork.SaveAsync();
        }

        public async Task<ArticleDto> GetArticleWIthCategoryNonDeletedAsync(Guid articleId)
        {
            var article = await _unitOfWork.GetRepository<Article>()
                .GetAsync(x => !x.IsDeleted && x.Id == articleId, x => x.Category, i=>i.Image);
            var map = _mapper.Map<ArticleDto>(article);
            return map;
        }

        public async Task<string> UpdateArticleAsync(ArticleUpdateDto articleUpdateDto)
        {
            var userEmail = _user.GetLoggedInEmail();
            var article = await _unitOfWork.GetRepository<Article>()
                .GetAsync(x => !x.IsDeleted && x.Id == articleUpdateDto.Id, x => x.Category, i => i.Image);
            if(articleUpdateDto.Photo !=null)
            {
                _imageHelper.Delete(article.Image.FileName);
                var imageUpload = await _imageHelper.Upload(articleUpdateDto.Title, articleUpdateDto.Photo,ImageType.Post);
                Image image = new(imageUpload.FullName, articleUpdateDto.Photo.ContentType, userEmail);
                await _unitOfWork.GetRepository<Image>().AddAsync(image);
                article.ImageId=image.Id;
            }
            _mapper.Map(articleUpdateDto, article);
            /*article.Title = articleUpdateDto.Title;
            article.Content = articleUpdateDto.Content;
            article.CategoryId = articleUpdateDto.CategoryId;*/
            article.ModifiedDate = DateTime.Now;
            article.ModifiedBy = userEmail;
            await _unitOfWork.GetRepository<Article>().UpdateAsunc(article);
            await _unitOfWork.SaveAsync();
            return article.Title;
        }

        public async Task<string> SafeDeleteArticleAsync(Guid articleId)
        {
            var userEmail = _user.GetLoggedInEmail();
            var article = await _unitOfWork.GetRepository<Article>().GetByGuidAsync(articleId);
            article.IsDeleted = true;
            article.DeletedDate = DateTime.Now;
            article.DeletedBy = userEmail;
            await _unitOfWork.GetRepository<Article>().UpdateAsunc(article);
            await _unitOfWork.SaveAsync();
            return article.Title;
        }
    }
}
