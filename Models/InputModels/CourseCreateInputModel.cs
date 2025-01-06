using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Customizations.DataAnnotations;
using MyCourse.Controllers;


namespace MyCourse.Models.InputModels
{
    public class CourseCreateInputModel
    {
        [Required(ErrorMessage = "Il titolo è obbligatorio"), 
        MinLength(10, ErrorMessage = "Il titolo dev'essere di almeno {1} caratteri"), 
        MaxLength(100, ErrorMessage = "Il titolo dev'essere di al massimo {1} caratteri"), 
        RegularExpression(@"^[\w\s\.]+$", ErrorMessage = "Titolo non valido"),
        Remote(action: nameof(CoursesController.IsTitleAvailable), controller: "Courses", ErrorMessage = "Il titolo esiste già"), // Data annotation che lancia una richiesta asincrona ajax, lanciando action e controller indicati
        NotText1234567890(ErrorMessage = "Il titolo non può essere 1234567890")] // Data annotation creata in maniera personalizzata
        public string Title { get; set; }
    }
}