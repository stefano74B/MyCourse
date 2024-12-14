using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyCourse.Controllers
{

    //se messo qui avrebbe effetto su tutte le action del controller
    // [ResponseCache(CacheProfileName = "Home")]
    public class HomeController : Controller
    {
        
        [ResponseCache(CacheProfileName = "Home")] // invochiamo il profile in startup.cs 
        public IActionResult Index()
        {
            //return Content("HOME");
            return View();
        }
    }
}