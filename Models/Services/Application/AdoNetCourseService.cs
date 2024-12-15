using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MyCourse.Models.ViewModels;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.Options;
using MyCourse.Models.Exceptions;
using MyCourse.Models.ValueObjects;

namespace MyCourse.Models.Services.Application
{
    public class AdoNetCourseService : ICourseService
    {
        private readonly ILogger<AdoNetCourseService> logger;
        private readonly IOptionsMonitor<CoursesOptions> coursesOptions;
        private readonly IDatabaseAccessor db;
        
        public AdoNetCourseService(ILogger<AdoNetCourseService> logger, IOptionsMonitor<CoursesOptions> coursesOptions, IDatabaseAccessor db)
        {
            //Usando ILoggerFactory possiamo definire anche il nome della categoria
            //this.logger = logger.CreateLogger("categoria");

            this.logger = logger;
            this.coursesOptions = coursesOptions;
            this.db = db; 
        }

        public async Task<List<CourseViewModel>> GetCoursesAsync(string search, int page, string orderby, bool ascending)
        {
            page = Math.Max(1, page);
            int limit = coursesOptions.CurrentValue.PerPage;
            int offset = (page - 1) * limit;

            var orderOptions = coursesOptions.CurrentValue.Order;
            if (!orderOptions.Allow.Contains(orderby))
            {
                orderby = orderOptions.By;
                ascending = orderOptions.Ascending;
            }

            //Decidere cosa estrarre dal db (componendo una query SQL)
            if (orderby == "CurrentPrice")
            {
                orderby = "CurrentPrice_Amount";
            }
            string direction = ascending ? "ASC" : "DESC";

            // {orderby} e {direction} hanno il codice (sql) perchè non sono da parametrizzare, sono parte integrante della stringa SQL, quindi abbiamo creato una classe per 'Sql' per gestirli in 'SqliteDatabaseAccessor.cs'
            FormattableString query = $"SELECT Id, Title, ImagePath, Author, Rating, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency FROM Courses WHERE Title LIKE {"%" + search + "%"} ORDER BY {(Sql) orderby} {(Sql) direction} LIMIT {limit} OFFSET {offset}";
            DataSet dataSet = await db.QueryAsync(query);
            var dataTable = dataSet.Tables[0];
            var courseList = new List<CourseViewModel>();
            foreach(DataRow courseRow in dataTable.Rows)
            {
                CourseViewModel course = CourseViewModel.FromDataRow(courseRow);
                courseList.Add(course);
            }
            return courseList;
        }

        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            // logging
            // Critical, Error, Warning, Information, Debug, Trace
            logger.LogInformation("Course {id} requested", id);

            FormattableString query = $@"SELECT Id, Title, Description, ImagePath, Author, Rating, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency FROM Courses WHERE Id={id}
            ; SELECT Id, Title, Description, Duration FROM Lessons WHERE CourseId={id}";
            DataSet dataSet = await db.QueryAsync(query);

            // Course
            var courseTable = dataSet.Tables[0];
            if (courseTable.Rows.Count != 1) {
                logger.LogWarning("Course {id} not found", id);
                throw new CourseNotFoundException(id);
            }
            var courseRow = courseTable.Rows[0];
            var courseDetailViewModel = CourseDetailViewModel.FromDataRow(courseRow);

            // Course lessons
            var lessonDatatable = dataSet.Tables[1];

            foreach(DataRow lessonRow in lessonDatatable.Rows) {
                LessonViewModel lessonViewModel = LessonViewModel.FromDataRow(lessonRow);
                courseDetailViewModel.Lessons.Add(lessonViewModel);
            }

            return courseDetailViewModel;
        }
    }
}