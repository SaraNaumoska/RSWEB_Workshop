using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacultyWebApp.Models
{
    public class Course
    {

        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public int Credits { get; set; }

        public int Semester { get; set; }

        [StringLength(100)]
        public string? Programme { get; set; }

        [StringLength(25)]
        [Display(Name = "Education level")]
        public string? EducationLevel { get; set; }

        public int? FirstTeacherId { get; set; }
        [Display(Name = "Professor")]
        public Teacher? FirstTeacher { get; set; }

        public int? SecondTeacherId { get; set; }


        [Display(Name = "Asisstant")]
        public Teacher? SecondTeacher { get; set; }


        public ICollection<Enrollment>? Students { get; set; }
    }
}