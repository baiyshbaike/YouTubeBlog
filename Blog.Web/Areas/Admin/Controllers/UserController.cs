using AutoMapper;
using Blog.Data.UnitOfWorks;
using Blog.Entity.DTOs.Articles;
using Blog.Entity.DTOs.Users;
using Blog.Entity.Entities;
using Blog.Entity.Enums;
using Blog.Service.Extensions;
using Blog.Service.Helpers.Images;
using Blog.Service.Services.Abstractioins;
using Blog.Web.ResultMessages;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IMapper mapper;
        private readonly IUserService userService;
        private readonly IToastNotification toastNotification;
        private readonly IValidator<AppUser> validator;
        public UserController(IMapper mapper ,IUserService userService,IToastNotification toastNotification,IValidator<AppUser> validator)
        {
            this.mapper = mapper;
            this.userService = userService;
            this.toastNotification = toastNotification;
            this.validator = validator;
        }
        public async Task<IActionResult> Index()
        {
            var result = await userService.GetAllUsersWithRoleAsync();
            return View(result);
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var roles = await userService.GetAllRolesAsync();
            return View(new UserAddDto { Roles = roles});
        }
        [HttpPost]
        public async Task<IActionResult> Add(UserAddDto userAddDto)
        {
            var roles = await userService.GetAllRolesAsync();
            var map = mapper.Map<AppUser>(userAddDto);
            var validation = await validator.ValidateAsync(map);
            if (ModelState.IsValid)
            {
                var result = await userService.CreateUserAsync(userAddDto);
                if (result.Succeeded)
                {
                    toastNotification.AddSuccessToastMessage(Messages.AppUser.Add(userAddDto.Email), new ToastrOptions() { Title = "successful!" });
                    return RedirectToAction("Index", "User", new { Area = "Admin" });
                }
                else
                {
                    result.AddToIdentityModelState(this.ModelState);
                    validation.AddToModelState(this.ModelState);
                    return View(new UserAddDto { Roles = roles });
                }
            }
            return View(new UserAddDto { Roles = roles });
        }
        [HttpGet]
        public async Task<IActionResult> Update(Guid userId)
        {
            var user = await userService.GetAppUserByIdAsync(userId);
            var roles = await userService.GetAllRolesAsync();
            var map = mapper.Map<UserUpdateDto>(user);
            map.Roles= roles;
            return View(map);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UserUpdateDto userUpdateDto)
        {
            var user = await userService.GetAppUserByIdAsync(userUpdateDto.Id);
            if (user != null)
            {
                var roles = await userService.GetAllRolesAsync();
                if (ModelState.IsValid)
                {
                    var map = mapper.Map(userUpdateDto, user);
                    var validation = validator.Validate(map);
                    if (validation.IsValid) 
                    {
                        user.UserName = userUpdateDto.Email;
                        user.SecurityStamp = Guid.NewGuid().ToString();
                        var result = await userService.UpdateUserAsync(userUpdateDto);
                        if (result.Succeeded)
                        {
                            toastNotification.AddSuccessToastMessage(Messages.AppUser.Update(userUpdateDto.Email), new ToastrOptions() { Title = "successful!" });
                            return RedirectToAction("Index", "User", new { Area = "Admin" });
                        }
                        else
                        {
                            result.AddToIdentityModelState(this.ModelState);
                            return View(new UserUpdateDto { Roles = roles });
                        }
                    }
                    else
                    {
                        validation.AddToModelState(this.ModelState);
                        return View(new UserUpdateDto { Roles = roles });
                    }
                }
            }
            return NotFound();
        }
        public async Task<IActionResult> Delete(Guid userId)
        {
            var result =  await userService.DeleteUserAsync(userId);
            if (result.idetntityResult.Succeeded)
            {
                toastNotification.AddSuccessToastMessage(Messages.AppUser.Delete(result.email), new ToastrOptions() { Title = "successful!" });
                return RedirectToAction("Index", "User", new { Area = "Admin" });
            }
            else
            {
                result.idetntityResult.AddToIdentityModelState(this.ModelState);
            }
            return NotFound();
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var profile = await userService.GetUserProfileAsync();
            return View(profile);
        }
        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileDto userProfileDto)
        {
            if(ModelState.IsValid)
            {
                var result = await userService.UserProfileUpdateAsync(userProfileDto);
                if (result)
                {
                    toastNotification.AddSuccessToastMessage("Profile update process completed", new ToastrOptions() { Title = "success" });
                    return RedirectToAction("Index", "User", new { Area = "Admin" });
                }
                else
                {
                    var profile = await userService.GetUserProfileAsync();
                    toastNotification.AddErrorToastMessage("Profile update process not completed", new ToastrOptions() { Title = "error" });
                    return View(profile);
                }
            }
            return NotFound();
        }
    }
}
