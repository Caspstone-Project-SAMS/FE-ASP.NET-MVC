﻿using Microsoft.AspNetCore.Mvc;

namespace WebMVCApplication.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}