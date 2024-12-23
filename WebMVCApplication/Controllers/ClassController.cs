using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebMVCApplication.Entities;
using WebMVCApplication.Models;
using WebMVCApplication.Services;

namespace WebMVCApplication.Controllers
{
    public class ClassController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISemesterService _semesterService;
        private readonly IClassService _classService;

        public ClassController(ILogger<HomeController> logger, ISemesterService semesterService, IClassService classService)
        {
            _logger = logger;
            _semesterService = semesterService;
            _classService = classService;
        }

        public async Task<IActionResult> Index()
        {
            ResultViewModel viewModel = new();
            viewModel.LoginUser = new LoginUser
            {
                UserRole = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value ?? string.Empty,
                UserName = User.Claims.FirstOrDefault(c => c.Type == "DisplayName")?.Value,
                Avatar = User.Claims.FirstOrDefault(c => c.Type == "Avatar")?.Value,
                Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty,
                Token = User.Claims.FirstOrDefault(c => c.Type == "token")?.Value ?? string.Empty
            };
            ViewData["Page"] = "ClassPage";

            string? bearerToken = User.Claims.FirstOrDefault(c => c.Type == "token")?.Value;
            string? lecturerId = null;
            if (viewModel.LoginUser.UserRole == "Lecturer")
                lecturerId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var getSemestersResult = await _semesterService.GetAllSemester(bearerToken);
            if (!getSemestersResult.IsSuccess)
            {
                viewModel.PopupNotification = new PopupNotification
                {
                    IsSuccess = false,
                    Title = "Get classes failed",
                    Description = new List<string> { "Can not get semester information" }
                };
            }
            else
            {
                var activeSemester = getSemestersResult.Result?.FirstOrDefault(s => s.Status == 2);
                var getClassesResult = await _classService.GetClasses(lecturerId, activeSemester?.Id, bearerToken);
                ViewData["Class-list"] = getClassesResult.Result ?? new();
                ViewData["Semester-list"] = getSemestersResult.Result ?? new();
                ViewData["Active-semester"] = activeSemester?.Id;
            }

            return View(viewModel);
        }
    }
}
