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
using MyCourse.Models.InputModels;

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
        
        public async Task<ListViewModel<CourseViewModel>> GetCoursesAsync(CourseListInputModel model)
        {
            IQueryable<Course> baseQuery = dbContext.Courses;

            switch(model.OrderBy)
            {
                case "Title":
                    if (model.Ascending)
                    {
                        baseQuery = baseQuery.OrderBy(course => course.Title);
                    }
                    else
                    {
                        baseQuery = baseQuery.OrderByDescending(course => course.Title);
                    }
                    break;
                case "Rating":
                    if (model.Ascending)
                    {
                        baseQuery = baseQuery.OrderBy(course => course.Rating);
                    }
                    else
                    {
                        baseQuery = baseQuery.OrderByDescending(course => course.Rating);
                    }
                    break;
                case "CurrentPrice":
                    if (model.Ascending)
                    {
                        baseQuery = baseQuery.OrderBy(course => course.CurrentPrice.Amount);
                    }
                    else
                    {
                        baseQuery = baseQuery.OrderByDescending(course => course.CurrentPrice.Amount);
                    }
                    break;
                case "Id":
                    if (model.Ascending)
                    {
                        baseQuery = baseQuery.OrderBy(course => course.Id);
                    }
                    else
                    {
                        baseQuery = baseQuery.OrderByDescending(course => course.Id);
                    }
                    break;
            }

            IQueryable<Course> queryLinq = baseQuery
                .Where(course => course.Title.Contains(model.Search))
                .AsNoTracking();

            List<CourseViewModel> courses = await queryLinq
                .Skip(model.Offset)
                .Take(model.Limit)
                .Select(course => CourseViewModel.FromEntity(course))
                .ToListAsync(); // La query viene eseguito esattamente in questo punto

            int totalCount = await queryLinq.CountAsync(); //conta i dati della query

            ListViewModel<CourseViewModel> result = new ListViewModel<CourseViewModel>
            {
                Results = courses,
                TotalCount = totalCount
            };

            return result;
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

            // per esempio potevamo anche fare così:
            // CourseDetailViewModel viewModel = await queryLinq.FirstOrDefaultAsync();
            // if (viewModel == null) 
            // {
            //     logger.LogWarning("Course {id} not found", id);
            //     throw new CourseNotFoundException(id);
            // }

            return viewModel;
        }

        public async Task<List<CourseViewModel>> GetBestRatingCoursesAsync()
        {
            CourseListInputModel inputModel = new CourseListInputModel(
                search: "",
                page: 1,
                orderby: "Rating",
                ascending: false,
                limit: coursesOptions.CurrentValue.InHome,
                orderOptions: coursesOptions.CurrentValue.Order
            );

            ListViewModel<CourseViewModel> result = await GetCoursesAsync(inputModel);
            return result.Results;
        }

        public async Task<List<CourseViewModel>> GetMostRecentCoursesAsync()
        {
            CourseListInputModel inputModel = new CourseListInputModel(
                search: "",
                page: 1,
                orderby: "Id",
                ascending: false,
                limit: coursesOptions.CurrentValue.InHome,
                orderOptions: coursesOptions.CurrentValue.Order
            );

            ListViewModel<CourseViewModel> result = await GetCoursesAsync(inputModel);
            return result.Results;
        }

        public async Task<CourseDetailViewModel> CreateCourseAsync(CourseCreateInputModel inputModel)
        {
            throw new NotImplementedException();
        }
    }
}