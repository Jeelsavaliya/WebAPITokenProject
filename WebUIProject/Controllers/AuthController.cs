using IdentityClass.Dto;
using Microsoft.AspNetCore.Mvc;
using WebUIProject.Service;

namespace WebUIProject.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly HttpClient _httpClient;
        public AuthController(IAuthService authService, HttpClient httpClient)
        {
            _authService = authService;
            _httpClient = httpClient;
        }
        public async Task<IActionResult> Login(LoginDto loginModel)
        {
            var response = await _authService.LoginAsync(loginModel);
            if (response != null)
            {
                // Handle successful login (store token, redirect, etc.)
                HttpContext.Session.SetString("JWTToken", response.Token);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Invalid login attempt");
            return View(loginModel);
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
