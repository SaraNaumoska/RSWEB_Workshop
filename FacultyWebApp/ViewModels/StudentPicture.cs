using FacultyWebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace FacultyWebApp.ViewModels
{
    public class StudentPicture
    {
        public Student? Student { get; set; }

        [Display(Name = "Upload picture")]
        public IFormFile? ProfilePictureFile { get; set; }

        [Display(Name = "Picture")]
        public string? ProfilePictureName { get; set; }
    }
}
