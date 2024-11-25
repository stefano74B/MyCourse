using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using MyCourse.Models.ViewModels;
using MyCourse.Models.Services.Infrastructure;

namespace MyCourse.Models.Services.Application
{
    public class AdoNetCourseService : ICourseService
    {
        private readonly IDatabaseAccessor db;
        public AdoNetCourseService(IDatabaseAccessor db)
        {
            this.db = db; 
        }

        public async Task<List<CourseViewModel>> GetCoursesAsync()
        {
            FormattableString query = $"SELECT Id, Title, ImagePath, Author, Rating, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency FROM Courses";
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
            FormattableString query = $@"SELECT Id, Title, Description, ImagePath, Author, Rating, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency FROM Courses WHERE Id={id}
            ; SELECT Id, Title, Description, Duration FROM Lessons WHERE CourseId={id}";
            DataSet dataSet = await db.QueryAsync(query);

            // Course
            var courseTable = dataSet.Tables[0];
            if (courseTable.Rows.Count != 1) {
                throw new InvalidOperationException($"Non è stata trovata 1 riga corrispondente per il corso {id}");
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