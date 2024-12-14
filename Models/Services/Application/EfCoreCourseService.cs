using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using MyCourse.Models.Entities;

namespace MyCourse.Models.Services.Application 
{
    public class EfCoreCourseService : ICourseService
    {
        private readonly MyCourseDbContext dbContext;

        public EfCoreCourseService(MyCourseDbContext dbContext)
        {
            this.dbContext = dbContext; 
        }
        
        public async Task<List<CourseViewModel>> GetCoursesAsync(string search)
        {
            search = search ?? "";
            IQueryable<CourseViewModel> queryLinq = dbContext.Courses
                .Where(course => course.Title.Contains(search))
                .AsNoTracking()
                .Select(course => CourseViewModel.FromEntity(course));

            List<CourseViewModel> courses = await queryLinq.ToListAsync(); // La query viene eseguito esattamente in questo punto

            return courses;
        }

        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
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