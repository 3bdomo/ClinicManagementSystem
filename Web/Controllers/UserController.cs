using AutoMapper;
using BLL.DTOs.User;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.ViewModel;

namespace Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string? roleFilter = null, string? searchQuery = null)
        {
            var result = await _userService.GetAllAsync();
            var users = result.IsSuccess ? result.Data : new List<UserDto>();

            if (!string.IsNullOrEmpty(roleFilter))
            {
                users = users.Where(u => u.UserRole.ToString().Equals(roleFilter, System.StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(searchQuery))
            {
                users = users.Where(u => u.FullName.Contains(searchQuery, System.StringComparison.OrdinalIgnoreCase) || 
                                         u.Email.Contains(searchQuery, System.StringComparison.OrdinalIgnoreCase));
            }

            var vm = new UserListViewModel
            {
                Users = _mapper.Map<IEnumerable<UserRowViewModel>>(users),
                RoleFilter = roleFilter,
                SearchQuery = searchQuery
            };

            return View(vm);
        }

        public IActionResult Create()
        {
            return View(new CreateUserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var dto = _mapper.Map<CreateUserDto>(vm);
            var result = await _userService.CreateAsync(dto);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "User created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(vm);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var result = await _userService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound();

            var vm = _mapper.Map<EditUserViewModel>(result.Data);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var dto = _mapper.Map<UpdateUserDto>(vm);
            var result = await _userService.UpdateAsync(dto);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "User updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var result = await _userService.ToggleActiveAsync(id);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "User status toggled successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ResetPassword(string id)
        {
            ViewBag.UserId = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id, string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
            {
                ModelState.AddModelError(string.Empty, "Password cannot be empty.");
                ViewBag.UserId = id;
                return View();
            }

            var result = await _userService.ResetPasswordAsync(id, newPassword);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Password reset successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Message);
            ViewBag.UserId = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userService.DeleteAsync(id);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
