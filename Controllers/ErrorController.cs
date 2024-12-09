using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using MyCourse.Models.Exceptions;

namespace MyCourse.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            // oggetto datoci grazie a app.UseExceptionHandler (Middleware) che contiene informazioni sull'errore
            // contiene 2 proprietà: error e path
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            switch (feature.Error)
            {
                case CourseNotFoundException exc:
                    ViewData["Title"] = "Corso non trovato";
                    Response.StatusCode = 404;
                    return View("CourseNotFound");

                    default:
                        ViewData["Title"] = "Errore";
                        return View(); 
            }


        }
    }
}