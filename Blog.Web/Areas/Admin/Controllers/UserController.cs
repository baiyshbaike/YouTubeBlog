using AutoMapper;
using Blog.Data.UnitOfWorks;
using Blog.Entity.DTOs.Articles;
using Blog.Entity.DTOs.Users;
using Blog.Entity.Entities;
using Blog.Entity.Enums;
using Blog.Service.Extensions;
using Blog.Service.Helpers.Images;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly IImageHelper imageHelper;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IToastNotification _toastNotification;
        private readonly IValidator<AppUser> _validator;
        private readonly SignInManager<AppUser> signInManager;

        public UserController(UserManager<AppUser> userManager,IImageHelper imageHelper,IUnitOfWork unitOfWork, IMapper mapper,RoleManager<AppRole> roleManager,IToastNotification toastNotification,IValidator<AppUser> validator,SignInManager<AppUser> signInManager)
        {
            this._userManager = userManager;
            this.imageHelper = imageHelper;
            this.unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._roleManager = roleManager;
            this._toastNotification = toastNotification;
            this._validator = validator;
            this.signInManager = signInManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var map = _mapper.Map<List<UserDto>>(users);
            foreach (var user in map)
            {
                var findUser = await _userManager.FindByIdAsync(user.Id.ToString());
                var role = string.Join("", await _userManager.GetRolesAsync(findUser));
                user.Role = role;
            }
            return View(map);
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(new UserAddDto { Roles = roles});
        }
        [HttpPost]
        public async Task<IActionResult> Add(UserAddDto userAddDto)
        {
            var map = _mapper.Map<AppUser>(userAddDto);
            var validation = await _validator.ValidateAsync(map);
            var roles = await _roleManager.Roles.ToListAsync();
            if (ModelState.IsValid)
            {
                map.UserName = userAddDto.Email;
                var result = await _userManager.CreateAsync(map,string.IsNullOrEmpty(userAddDto.Password)? "": userAddDto.Password);
                if (result.Succeeded)
                {
                    var findRole = await _roleManager.FindByIdAsync(userAddDto.RoleId.ToString());
                    await _userManager.AddToRoleAsync(map, findRole.ToString());
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
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var roles = await _roleManager.Roles.ToListAsync();
            var map = _mapper.Map<UserUpdateDto>(user);
            map.Roles= roles;
            return View(map);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UserUpdateDto userUpdateDto)
        {
            var user = await _userManager.FindByIdAsync(userUpdateDto.Id.ToString());
            if (user != null)
            {
                var userRole = string.Join("", await _userManager.GetRolesAsync(user));
                var roles = await _roleManager.Roles.ToListAsync();
                if (ModelState.IsValid)
                {
                    var map = _mapper.Map(userUpdateDto, user);
                    var validation = _validator.Validate(map);
                    if (validation.IsValid) 
                    {
                        user.UserName = userUpdateDto.Email;
                        user.SecurityStamp = Guid.NewGuid().ToString();
                        var result = await _userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            await _userManager.RemoveFromRoleAsync(user, userRole);
                            var findRole = await _roleManager.FindByIdAsync(userUpdateDto.RoleId.ToString());
                            await _userManager.AddToRoleAsync(user, findRole.Name);
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
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var result =  await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _toastNotification.AddSuccessToastMessage(Messages.AppUser.Delete(user.Email), new ToastrOptions() { Title = "successful!" });
                return RedirectToAction("Index", "User", new { Area = "Admin" });
            }
            else
            {
                result.AddToIdentityModelState(this.ModelState);
            }
            return NotFound();
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var getImage = await unitOfWork.GetRepository<AppUser>().GetAsync(x => x.Id == user.Id, x => x.Image);
            var map = _mapper.Map<UserProfileDto>(user);
            map.Image.FileName = getImage.Image.FileName;
            return View(map);
        }
        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileDto userProfileDto)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if(ModelState.IsValid)
            {
                var isVeriFied = await _userManager.CheckPasswordAsync(user, userProfileDto.CurrentPassword);
                if(isVeriFied && userProfileDto.NewPassword != null && userProfileDto.Photo != null)
                {
                    var result = await _userManager.ChangePasswordAsync(user,userProfileDto.CurrentPassword,userProfileDto.NewPassword);
                    if(result.Succeeded)
                    {
                        await _userManager.UpdateSecurityStampAsync(user);
                        await signInManager.SignOutAsync();
                        await signInManager.PasswordSignInAsync(user, userProfileDto.NewPassword, true, false);
                        user.FirstName = userProfileDto.FirstName; 
                        user.LastName = userProfileDto.LastName;
                        user.PhoneNumber = userProfileDto.PhoneNumber;
                        var imageUpload = await imageHelper.Upload($"{userProfileDto.FirstName}{userProfileDto.LastName}", userProfileDto.Photo, ImageType.User);
                        Image image = new(imageUpload.FullName, userProfileDto.Photo.ContentType, userProfileDto.Email);
                        await unitOfWork.GetRepository<Image>().AddAsync(image);
                        user.ImageId= image.Id; 
                        await _userManager.UpdateAsync(user);
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
                    await _userManager.UpdateSecurityStampAsync(user);
                    user.FirstName = userProfileDto.FirstName; 
                    user.LastName = userProfileDto.LastName;
                    user.PhoneNumber = userProfileDto.PhoneNumber;
                    var imageUpload = await imageHelper.Upload($"{userProfileDto.FirstName}{userProfileDto.LastName}", userProfileDto.Photo, ImageType.User);
                    Image image = new(imageUpload.FullName, userProfileDto.Photo.ContentType, userProfileDto.Email);
                    await unitOfWork.GetRepository<Image>().AddAsync(image);
                    user.ImageId = image.Id;
                    await _userManager.UpdateAsync(user);
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
