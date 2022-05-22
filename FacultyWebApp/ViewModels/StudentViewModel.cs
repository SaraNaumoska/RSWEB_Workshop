using FacultyWebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FacultyWebApp.ViewModels
{
    public class StudentViewModel
    {
        public IList<Student> Students { get; set; }
        public SelectList IDs { get; set; }
        public int studentIndex { get; set; }
    }
}
