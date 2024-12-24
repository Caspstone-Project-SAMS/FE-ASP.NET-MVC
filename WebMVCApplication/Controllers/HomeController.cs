using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;
using WebMVCApplication.Common;
using WebMVCApplication.Models;
using WebMVCApplication.Services;

namespace WebMVCApplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IScheduleService _scheduleService;
        private readonly ISemesterService _semesterService;

        public HomeController(ILogger<HomeController> logger, IScheduleService scheduleService, ISemesterService semesterService)
        {
            _logger = logger;
            _scheduleService = scheduleService;
            _semesterService = semesterService;
        }

        [Route("/")]
        [Route("/home")]
        public async Task<IActionResult> Index()
        {
            var viewModel = JsonConvert.DeserializeObject<ResultViewModel>(TempData["ResultViewModel"] as string ?? "");
            if(viewModel is null) viewModel = new ResultViewModel();
            viewModel.LoginUser = new LoginUser
            {
                UserRole = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value ?? string.Empty,
                UserName = User.Claims.FirstOrDefault(c => c.Type == "DisplayName")?.Value,
                Avatar = User.Claims.FirstOrDefault(c => c.Type == "Avatar")?.Value
            };

            string? bearerToken = User.Claims.FirstOrDefault(c => c.Type == "token")?.Value;
            string? lecturerId = null;
            if (viewModel.LoginUser.UserRole == "Lecturer")
                lecturerId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // get datetime info
            var vnDateTime = ServerDateTime.GetVnDateTime();
            var dayOfWeek = (int)vnDateTime.DayOfWeek;
            if (dayOfWeek == 0)
                dayOfWeek = 7;
            var startDate = vnDateTime.AddDays((dayOfWeek - 1) * -1);
            var endDate = vnDateTime.AddDays(7 - dayOfWeek);

            // Get schedules in a week
            var currentDateOnly = DateOnly.FromDateTime(vnDateTime);
            var getSemestersResult = await _semesterService.GetAllSemester(bearerToken);
            if (!getSemestersResult.IsSuccess)
            {
                viewModel.PopupNotification = new PopupNotification
                {
                    IsSuccess = false,
                    Title = "Get schedules failed",
                    Description = new List<string> { "Can not get semester information" }
                };
                return View(viewModel);
            }
            var currentSemester = getSemestersResult.Result?
                .Where(s => s.StartDate <= currentDateOnly && currentDateOnly <= s.EndDate)
                .FirstOrDefault();

            var getSchedulesResult = await _scheduleService.GetSchedules(lecturerId, currentSemester?.Id, bearerToken, startDate, endDate, 1, 10, 50);
            if (!getSchedulesResult.IsSuccess)
            {
                viewModel.PopupNotification = new PopupNotification
                {
                    IsSuccess = false,
                    Title = "Get schedules failed",
                };
            }

            ViewData["Schedules"] = getSchedulesResult.Result;
            ViewData["VN-DateTime"] = vnDateTime;
            ViewData["Start-date"] = DateOnly.FromDateTime(startDate);
            ViewData["End-date"] = DateOnly.FromDateTime(endDate);
            ViewData["Current-semester-code"] = currentSemester?.SemesterCode ?? string.Empty;
            ViewData["Page"] = "HomePage";

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
