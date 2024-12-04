using System;
using System.Collections.Generic;

namespace MyCourse.Models.Entities
{
    public partial class Lesson
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TimeSpan Duration { get; set; } // 00:00:00

        public virtual Course Course { get; set; }
    }
}
