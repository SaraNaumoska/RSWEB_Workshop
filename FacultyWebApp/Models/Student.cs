using System.ComponentModel.DataAnnotations;

namespace FacultyWebApp.Models
{
    public class Student
    {
        [Required]
        [Key]
        public long Id { get; set; }

        [Required]
        [Display(Name = "Student ID")]
        [StringLength(10)]
        public string? studentId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(30, MinimumLength = 5)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(30, MinimumLength = 5)]
        public string LastName { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }


        [Required]
        [Display(Name = "Acquired credits")]
        public int AcquiredCredits { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "Enrollment Date")]
        public DateTime EnrollmentDate { get; set; }


        [Display(Name = "Current semester")]
        public int CurrentSemester { get; set; }

        [Display(Name = "Education level")]
        [StringLength(25)]
        public string? EducationLevel { get; set; }


        public ICollection<Enrollment>? Courses { get; set; }

        public string? profilePicture;

    }
}