using FacultyWebApp.Areas.Identity.Data;
using FacultyWebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacultyWebApp.Models
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<FacultyWebAppUser>>();
            IdentityResult roleResult;


            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            FacultyWebAppUser user = await UserManager.FindByEmailAsync("admin@faxapp.com");
            if (user == null)
            {
                var User = new FacultyWebAppUser();
                User.Email = "admin@faxapp.com";
                User.UserName = "admin@faxapp.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }

            //Add Teacher Role
            roleCheck = await RoleManager.RoleExistsAsync("Teacher");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Teacher")); }
            user = await UserManager.FindByEmailAsync("teacher@faxapp.com");
            if (user == null)
            {
                var User = new FacultyWebAppUser();
                User.Email = "teacher@faxapp.com";
                User.UserName = "teacher@faxapp.com";
                string userPWD = "Teacher123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Teacher
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Teacher"); }
            }

            //Add Student Role
            roleCheck = await RoleManager.RoleExistsAsync("Student");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Student")); }
            user = await UserManager.FindByEmailAsync("student@faxapp.com");
            if (user == null)
            {
                var User = new FacultyWebAppUser();
                User.Email = "student@faxapp.com";
                User.UserName = "student@faxapp.com";
                string userPWD = "Student123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Student
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Student"); }
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new FacultyWebAppContext(
            serviceProvider.GetRequiredService<
            DbContextOptions<FacultyWebAppContext>>()))
            {

                CreateUserRoles(serviceProvider).Wait();


                // Look for any movies.
                if (context.Course.Any() || context.Student.Any() || context.Teacher.Any())
                {
                    return; // DB has been seeded
                }
                context.Teacher.AddRange(
                new Teacher { Id = 1, FirstName = "Jana", LastName = "M.", Degree = "FEIT", AcademicRank = "Doktor" },
                new Teacher { Id = 2, FirstName = "Sara", LastName = "R.", Degree = "FINKI", AcademicRank = "Docent" },
                new Teacher { Id = 3, FirstName = "Jovan", LastName = "H.", Degree = "Medicina", AcademicRank = "Doktor" }
                );
                context.SaveChanges();
                context.Student.AddRange(
                new Student { studentId = " ", Id = 1, FirstName = "Robert", LastName = "C.", AcquiredCredits = 50 },
                new Student { studentId = " ", Id = 2, FirstName = "Natalija", LastName = "R.", AcquiredCredits = 45 },
                new Student { studentId = " ", Id = 3, FirstName = "Zivko", LastName = "F.", AcquiredCredits = 45 },
                new Student { studentId = " ", Id = 4, FirstName = "Valentin", LastName = "M.", AcquiredCredits = 45 },
                new Student { studentId = " ", Id = 5, FirstName = "Nikolina", LastName = "A.", AcquiredCredits = 45 },
                new Student { studentId = " ", Id = 6, FirstName = "Magdalena", LastName = "D.", AcquiredCredits = 45 },
                new Student { studentId = " ", Id = 7, FirstName = "Aleksandar", LastName = "N.", AcquiredCredits = 45 },
                new Student { studentId = " ", Id = 8, FirstName = "Borislav", LastName = "M.", AcquiredCredits = 45 }
                );
                context.SaveChanges();
                context.Course.AddRange(
                new Course
                {
                    //Id = 1,
                    Title = "Mathematics 3",
                    Credits = 6,
                    Semester = 3,
                    Programme = "TKII",
                    FirstTeacherId = context.Teacher.Single(d => d.FirstName == "Jana" && d.LastName == "M.").Id,

                },
                new Course
                {
                    //Id = 2,
                    Title = "Information Theory",
                    Credits = 8,
                    Semester = 6,
                    Programme = "KSIAR",
                    FirstTeacherId = context.Teacher.Single(d => d.FirstName == "Sara" && d.LastName == "R.").Id,
                    SecondTeacherId = context.Teacher.Single(d => d.FirstName == "Jovana" && d.LastName == "H.").Id
                },
                new Course
                {
                    //Id = 3,
                    Title = "Programming and algorithms",
                    Credits = 4,
                    Semester = 8,
                    Programme = "KTI",
                    FirstTeacherId = context.Teacher.Single(d => d.FirstName == "Jovan" && d.LastName == "H.").Id,

                }
                );
                context.SaveChanges();
                context.Enrollment.AddRange
                (
                new Enrollment { studentId = 1, CourseId = 1 },
                new Enrollment { studentId = 2, CourseId = 1 },
                new Enrollment { studentId = 3, CourseId = 1 },
                new Enrollment { studentId = 4, CourseId = 2 },
                new Enrollment { studentId = 5, CourseId = 2 },
                new Enrollment { studentId = 6, CourseId = 2 },
                new Enrollment { studentId = 4, CourseId = 3 },
                new Enrollment { studentId = 5, CourseId = 3 },
                new Enrollment { studentId = 6, CourseId = 3 }
                );
                context.SaveChanges();
            }
        }
    }
}