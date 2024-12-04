using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MyCourse.Models.Services.Application 
{
    public class EfCoreCourseService : ICourseService
    {
        private readonly MyCourseDbContext dbContext;

        public EfCoreCourseService(MyCourseDbContext dbContext)
        {
            this.dbContext = dbContext; 
        }
        
        public async Task<List<CourseViewModel>> GetCoursesAsync()
        {
            List<CourseViewModel> courses =  await dbContext.Courses.Select(course =>
            new CourseViewModel {
                Id = course.Id,
                Title = course.Title,
                ImagePath = course.ImagePath,
                Author = course.Author,
                Rating = course.Rating,
                CurrentPrice = course.CurrentPrice,
                FullPrice = course.FullPrice
            })
            .ToListAsync();

            return courses;
        }

        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            throw new System.NotImplementedException();
        }

    }
}