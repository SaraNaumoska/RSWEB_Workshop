#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FacultyWebApp.Data;
using FacultyWebApp.Models;
using FacultyWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace FacultyWebApp.Controllers
{
    public class StudentsController : Controller
    {
        private readonly FacultyWebAppContext _context;

        public StudentsController(FacultyWebAppContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index(string studentID, string fullname)
        {
            IQueryable<Student> Students = _context.Student.AsQueryable();

            if (!string.IsNullOrEmpty(fullname))
            {
                if (fullname.Contains(" "))
                {
                    string[] names = fullname.Split(" ");
                    Students = Students.Where(s => s.FirstName.Contains(names[0]) || s.LastName.Contains(names[1]));
                }
                else
                {
                    Students = Students.Where(s => s.FirstName.Contains(fullname) || s.LastName.Contains(fullname));
                }
            }

            if (!string.IsNullOrEmpty(studentID))
            {
                Students = Students.Where(s => s.studentId.Contains(studentID));
            }

            var StudentFilter = new StudentsFilter
            {
                Students = await Students.ToListAsync(),
            };

            return View(StudentFilter);
        }
        // GET: Students/Details/5
        [Authorize]
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student.FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }
            StudentPicture viewmodel = new StudentPicture
            {
                Student = student,
                ProfilePictureName = student.profilePicture
            };

            return View(viewmodel);
        }

        // GET: Students/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["Courses"] = new SelectList(_context.Set<Course>(), "CourseId", "Title");

            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,studentId,FirstName,LastName,AcquiredCredits,EnrollmentDate,CurrentSemester,EducationLevel")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = _context.Student.Where(x => x.Id == id).Include(x => x.Courses).First();
            if (student == null)
            {
                return NotFound();
            }

            var courses = _context.Course.AsEnumerable();
            courses = courses.OrderBy(s => s.Title);

            EnrollCourses viewmodel = new EnrollCourses
            {
                student = student,
                coursesEnrolledList = new MultiSelectList(courses, "CourseId", "Title"),
                selectedCourses = student.Courses.Select(x => x.CourseId)
            };

            ViewData["Courses"] = new SelectList(_context.Set<Course>(), "CourseId", "Title");
            return View(viewmodel);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(long id, EnrollCourses viewmodel)
        {
            if (id != viewmodel.student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewmodel.student);
                    await _context.SaveChangesAsync();

                    var student = _context.Student.Where(x => x.Id == id).First();

                    IEnumerable<int> selectedCourses = viewmodel.selectedCourses;
                    if (selectedCourses != null)
                    {
                        IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => !selectedCourses.Contains(s.CourseId) && s.studentId == id);
                        _context.Enrollment.RemoveRange(toBeRemoved);

                        IEnumerable<int> existEnrollments = _context.Enrollment.Where(s => selectedCourses.Contains(s.CourseId) && s.studentId == id).Select(s => s.CourseId);
                        IEnumerable<int> newEnrollments = selectedCourses.Where(s => !existEnrollments.Contains(s));

                        foreach (int courseId in newEnrollments)
                            _context.Enrollment.Add(new Enrollment { studentId = id, CourseId = courseId, Semester = viewmodel.semester, Year = (int)viewmodel.year });

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => s.studentId == id);
                        _context.Enrollment.RemoveRange(toBeRemoved);
                        await _context.SaveChangesAsync();
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(viewmodel.student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewmodel);
        }

        // GET: Students/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var student = await _context.Student.FindAsync(id);
            _context.Student.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(long id)
        {
            return _context.Student.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Enrolled(int? id)
        {
            if (id == null) 
            {
                return NotFound();
            }

            var course = await _context.Course
                .FirstOrDefaultAsync(m => m.Id == id);

            IQueryable<Student> studentQuery = _context.Enrollment.Where(x => x.CourseId == id).Select(x => x.student);
            await _context.SaveChangesAsync();
            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Message = course.Title;
            var studentVM = new StudentViewModel
            {
                Students = await studentQuery.ToListAsync(),
            };

            return View(studentVM);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditPicture(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = _context.Student.Where(x => x.Id == id).Include(x => x.Courses).First();
            if (student == null)
            {
                return NotFound();
            }

            var courses = _context.Course.AsEnumerable();
            courses = courses.OrderBy(s => s.Title);

            StudentPicture viewmodel = new StudentPicture
            {
                Student = student,
                ProfilePictureName = student.profilePicture
            };

            return View(viewmodel);
        }

        // POST: Students/EditPicture/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditPicture(long id, StudentPicture viewmodel)
        {
            if (id != viewmodel.Student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (viewmodel.ProfilePictureFile != null)
                    {
                        string uniqueFileName = UploadedFile(viewmodel);
                        viewmodel.Student.profilePicture = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.Student.profilePicture = viewmodel.ProfilePictureName;
                    }

                    _context.Update(viewmodel.Student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(viewmodel.Student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = viewmodel.Student.Id });
            }
            return View(viewmodel);
        }
        private string UploadedFile(StudentPicture viewmodel)
        {
            string uniqueFileName = null;

            if (viewmodel.ProfilePictureFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/profilePictures");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewmodel.ProfilePictureFile.FileName);
                string fileNameWithPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewmodel.ProfilePictureFile.CopyTo(stream);
                }
            }
            return uniqueFileName;
        }
    }
}
