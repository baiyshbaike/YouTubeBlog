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
        private readonly RoleManager<AppRole> roleManager;
        private readonly UserManager<AppUser> userManager;
        private readonly IMapper mapper;
        private readonly IUserService userService;
        private readonly IImageHelper imageHelper;
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        private readonly IValidator<AppUser> _validator;
        private readonly SignInManager<AppUser> signInManager;

        public UserController(IUnitOfWork unitOfWork , RoleManager<AppRole> roleManager ,UserManager<AppUser> userManager, IMapper mapper ,IUserService userService,IImageHelper imageHelper,IToastNotification toastNotification,IValidator<AppUser> validator,SignInManager<AppUser> signInManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.mapper = mapper;
            this.userService = userService;
            this.imageHelper = imageHelper;
            this.unitOfWork = unitOfWork;
            this._toastNotification = toastNotification;
            this._validator = validator;
            this.signInManager = signInManager;
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
            var validation = await _validator.ValidateAsync(map);
            if (ModelState.IsValid)
            {
                var result = await userService.CreateUserAsync(userAddDto);
                if (result.Succeeded)
                {
                    _toastNotification.AddSuccessToastMessage(Messages.AppUser.Add(userAddDto.Email), new ToastrOptions() { Title = "successful!" });
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
                    var validation = _validator.Validate(map);
                    if (validation.IsValid) 
                    {
                        user.UserName = userUpdateDto.Email;
                        user.SecurityStamp = Guid.NewGuid().ToString();
                        var result = await userService.UpdateUserAsync(userUpdateDto);
                        if (result.Succeeded)
                        {
                            _toastNotification.AddSuccessToastMessage(Messages.AppUser.Update(userUpdateDto.Email), new ToastrOptions() { Title = "successful!" });
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
                _toastNotification.AddSuccessToastMessage(Messages.AppUser.Delete(result.email), new ToastrOptions() { Title = "successful!" });
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
            var user = await userManager.GetUserAsync(HttpContext.User);
            var getImage = await unitOfWork.GetRepository<AppUser>().GetAsync(x => x.Id == user.Id, x => x.Image);
            var map = mapper.Map<UserProfileDto>(user);
            map.Image.FileName = getImage.Image.FileName;
            return View(map);
        }
        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileDto userProfileDto)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            if(ModelState.IsValid)
            {
                var isVeriFied = await userManager.CheckPasswordAsync(user, userProfileDto.CurrentPassword);
                if(isVeriFied && userProfileDto.NewPassword != null && userProfileDto.Photo != null)
                {
                    var result = await userManager.ChangePasswordAsync(user,userProfileDto.CurrentPassword,userProfileDto.NewPassword);
                    if(result.Succeeded)
                    {
                        await userManager.UpdateSecurityStampAsync(user);
                        await signInManager.SignOutAsync();
                        await signInManager.PasswordSignInAsync(user, userProfileDto.NewPassword, true, false);
                        user.FirstName = userProfileDto.FirstName; 
                        user.LastName = userProfileDto.LastName;
                        user.PhoneNumber = userProfileDto.PhoneNumber;
                        var imageUpload = await imageHelper.Upload($"{userProfileDto.FirstName}{userProfileDto.LastName}", userProfileDto.Photo, ImageType.User);
                        Image image = new(imageUpload.FullName, userProfileDto.Photo.ContentType, userProfileDto.Email);
                        await unitOfWork.GetRepository<Image>().AddAsync(image);
                        user.ImageId= image.Id; 
                        await userManager.UpdateAsync(user);
                        await unitOfWork.SaveAsync();
                        _toastNotification.AddSuccessToastMessage("success");
                        return View();
                    }
                    else
                    {
                        result.AddToIdentityModelState(ModelState); return View();
                    }
                }
                else if(isVeriFied && userProfileDto.Photo != null)
                {
                    await userManager.UpdateSecurityStampAsync(user);
                    user.FirstName = userProfileDto.FirstName; 
                    user.LastName = userProfileDto.LastName;
                    user.PhoneNumber = userProfileDto.PhoneNumber;
                    var imageUpload = await imageHelper.Upload($"{userProfileDto.FirstName}{userProfileDto.LastName}", userProfileDto.Photo, ImageType.User);
                    Image image = new(imageUpload.FullName, userProfileDto.Photo.ContentType, userProfileDto.Email);
                    await unitOfWork.GetRepository<Image>().AddAsync(image);
                    user.ImageId = image.Id;
                    await userManager.UpdateAsync(user);
                    await unitOfWork.SaveAsync();
                    _toastNotification.AddSuccessToastMessage("success");
                    return View();
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("error");
                    return View();
                }
            }
            return View();
        }
    }
}
