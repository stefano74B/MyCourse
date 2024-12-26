using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Models.ViewModels;

namespace MyCourse.Customizations.ViewComponents
{
    public class PaginationBarViewComponent : ViewComponent
    {
        
        public IViewComponentResult Invoke(IPaginationInfo model)
        {
            return View(model);
        }

    }
}