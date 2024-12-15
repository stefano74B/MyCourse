using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels;
using MyCourse.Models.Options;
using MyCourse.Models.Entities;

namespace MyCourse.Models.Services.Application 
{
    public class EfCoreCourseService : ICourseService
    {
        private readonly ILogger<AdoNetCourseService> logger;
        private readonly IOptionsMonitor<CoursesOptions> coursesOptions;
        private readonly MyCourseDbContext dbContext;

        public EfCoreCourseService(ILogger<AdoNetCourseService> logger, IOptionsMonitor<CoursesOptions> coursesOptions, MyCourseDbContext dbContext)
        {
            this.logger = logger;
            this.coursesOptions = coursesOptions;
            this.dbContext = dbContext; 
        }
        
        public async Task<List<CourseViewModel>> GetCoursesAsync(string search, int page)
        {
            search = search ?? "";
            page = Math.Max(1, page);
            int limit = coursesOptions.CurrentValue.PerPage;
            int offset = (page - 1) * limit;

            IQueryable<CourseViewModel> queryLinq = dbContext.Courses
                .Where(course => course.Title.Contains(search))
                .Skip(offset)
                .Take(limit)
                .AsNoTracking()
                .Select(course => CourseViewModel.FromEntity(course));

            List<CourseViewModel> courses = await queryLinq.ToListAsync(); // La query viene eseguito esattamente in questo punto

            return courses;
        }

        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            // logging
            // Critical, Error, Warning, Information, Debug, Trace
            logger.LogInformation("Course {id} requested", id);

            IQueryable<CourseDetailViewModel> queryLinq = dbContext.Courses
                .AsNoTracking() // Invocandolo rinunciamo al tracciamento delle modifiche in cambio di più velocità, usarlo quindi quando dobbiamo soltanto leggere e non modificare i dati
                .Include(course => course.Lessons)
                .Where(course => course.Id == id)
                .Select(course => CourseDetailViewModel.FromEntity(course));

                CourseDetailViewModel viewModel = await queryLinq.SingleAsync(); // Restituisce primo elemento dell'elenco, ma se ne contiene 0 o più di 1 allora solleva un'eccezione
                // .FirstAsync(); // Restituisce il primo elemento dell'elenco, ma solo se è vuoto solleva un'eccezione, se ne contiene più di 1 prende effettivamente solo il primo
                // .SingleOrDefaultAsync(); //Tollera il fatto che l'elenco sia vuoto, e in quel caso restituisce null (che è il default dei tipi complessi come in questo caso), oppure se l'elemento contiene più di 1 elemento solleva un'eccezione
                // .FirstOrDefaultAsync(); // Restituisce null se l'elenco è vuoto e non solleva mai un'eccezione

            return viewModel;
        }

    }
}