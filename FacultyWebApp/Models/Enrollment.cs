using System.ComponentModel.DataAnnotations;

namespace FacultyWebApp.Models
{
    public class Enrollment
    {
        [Required]
        public long Id { get; set; }

        [Required]

        public int CourseId { get; set; }
        [Display(Name = "Course")]
        public Course? course { get; set; }

        [Required]
        public long studentId { get; set; }
        [Display(Name = "Student")]
        public Student? student { get; set; }

        [StringLength(10)]
        public string? Semester { get; set; }

        public int Year { get; set; }

        public int Grade { get; set; }

        [StringLength(255)]
        public string? SeminalUrl { get; set; }

        [StringLength(255)]
        public string? ProjectUrl { get; set; }


        public int ExamPoints { get; set; }

        public int SeminalPoints { get; set; }

        public int ProjectPoints { get; set; }

        public int AdditionalPoints { get; set; }

        [DataType(DataType.Date)]
        public DateTime FinishDate { get; set; }

    }
}