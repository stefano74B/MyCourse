using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Models.Services.Application;
using MyCourse.Models.ViewModels;

namespace MyCourse.Controllers
{

    //se messo qui avrebbe effetto su tutte le action del controller
    // [ResponseCache(CacheProfileName = "Home")]
    public class HomeController : Controller
    {
        
        // [ResponseCache(CacheProfileName = "Home")] // invochiamo il profile in startup.cs 
        public async Task<IActionResult> Index([FromServices] ICachedCourseService courseService)
        {
            //return Content("HOME");
            ViewData["Title"] = "Benvenuti in MyCourse!";
            List<CourseViewModel> bestRatingCourses = await courseService.GetBestRatingCoursesAsync();
            List<CourseViewModel> mostRecentCourses = await courseService.GetMostRecentCoursesAsync();

            HomeViewModel viewModel = new HomeViewModel
            {
                BestRatingCourses = bestRatingCourses,
                MostRecentCourses = mostRecentCourses
            };

            return View(viewModel);
        }
    }
}