using Blog.Data.Context;
using Blog.Data.Repositories.Abstractioins;
using Blog.Data.Repositories.Concretes;
using Blog.Data.UnitOfWorks;
using Blog.Service.Services.Abstractioins;
using Blog.Service.Services.Contrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Blog.Service.FluentValidations;
using Blog.Service.Helpers.Images;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;

namespace Blog.Service.Extensions
{
    public static class SerivceLayerExtension
    {
        /*[Obsolete]*/
        public static IServiceCollection LoadServiceLayerExtension(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IImageHelper, ImagesHelper>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddAutoMapper(assembly);
            services.AddControllersWithViews().AddFluentValidation(opt =>
            {
                opt.RegisterValidatorsFromAssemblyContaining<ArticleValidator>();
                opt.DisableDataAnnotationsValidation = true;
                opt.ValidatorOptions.LanguageManager.Culture = new CultureInfo("ru");
            });
            return services;
        }
    }
}
