using FacultyWebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FacultyWebApp.ViewModels
{
    public class EnrollmentsViewModel
    {
        public List<Enrollment>? enrollments { get; set; }
        public SelectList? yearlist { get; set; }
        public int? year { get; set; }
    }
}
