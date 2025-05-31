using Microsoft.AspNetCore.Mvc;
using StudyBuddyMVC.Services;
using StudyBuddyMVC.DTOs;
using StudyBuddyMVC.Extensions;

namespace StudyBuddyMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserApiService _userService;


        public UserController(IUserApiService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {

            return View();
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var success = await _userService.RegisterAsync(dto);
            if (!success)
            {
                ModelState.AddModelError("", "Registration failed.");
                return View(dto);
            }

            return RedirectToAction("Index");
        }

        public IActionResult LogIn() => View();

        [HttpPost]
        public async Task<IActionResult> LogIn(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var tekenResponse = await _userService.LoginAsync(dto);

            if (tekenResponse.Token == null || tekenResponse.UserId == null)
            {
                ModelState.AddModelError("", "Log in failed.");
                return View(dto);
            }


            HttpContext.Session.SetString("token", tekenResponse.Token);
            HttpContext.Session.SetString("userId", tekenResponse.UserId);


            var user = await _userService.GetUserByIdAsync(tekenResponse.UserId);            

            HttpContext.Session.SetObject("loggedUser", user);

            return RedirectToAction("Index");
        }
    }

}
