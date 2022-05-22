using FacultyWebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace FacultyWebApp.ViewModels
{
    public class StudentEditViewModel
    {
        public Enrollment Enrollment { get; set; }

        [Display(Name = "Seminal File")]
        public IFormFile? SeminalUrlFile { get; set; }
        public string? SeminalUrlName { get; set; }
    }
}
