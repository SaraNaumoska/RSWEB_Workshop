using FacultyWebApp.Models;

namespace FacultyWebApp.ViewModels
{
    public class StudentsFilter
    {
        public IList<Student>? Students { get; set; }

        public string? FullName { get; set; }

        public string? studentID { get; set; }
    }
}
