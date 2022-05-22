using FacultyWebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FacultyWebApp.ViewModels
{
    public class CourseViewModel
    {
        public List<Course> Courses { get; set; }
        public SelectList Semester { get; set; }
        public SelectList Programmes { get; set; }
        public string CourseSemester { get; set; }
        public string CourseProgramme { get; set; }
        public string SearchString { get; set; }
    }
}
