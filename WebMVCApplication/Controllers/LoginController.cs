using Microsoft.AspNetCore.Mvc;
using WebMVCApplication.Models;
using WebMVCApplication.Services;

namespace WebMVCApplication.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAuthService _authService;

        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Index()
        {
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm]string Username, [FromForm]string Password)
        {
            var result = await _authService.Login(Username, Password);
            if (result.IsSuccess)
            {
                var loginResultViewModel = new ResultViewModel
                {
                    PopupNotification = new PopupNotification
                    {
                        IsSuccess = result.IsSuccess,
                        Title = result.Title ?? "Success",
                    }
                };
                return RedirectToAction("index", "home", loginResultViewModel);
            }
            else
            {
                return View(new ResultViewModel
                {
                    PopupNotification = new PopupNotification
                    {
                        IsSuccess = result.IsSuccess,
                        Title = result.Title ?? "Failed",
                        Description = result.Errors ?? new string[0]
                    }
                });
            }
        }
    }
}
