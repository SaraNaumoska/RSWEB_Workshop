using FacultyWebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace FacultyWebApp.ViewModels
{
    public class TeacherPicture
    {
        public Teacher? teacher { get; set; }

        [Display(Name = "Upload picture")]
        public IFormFile? profilePictureFile { get; set; }

        [Display(Name = "Picture name")]
        public string? profilePictureName { get; set; }
    }
}
