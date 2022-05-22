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
    public class CoursesController : Controller
    {
        private readonly FacultyWebAppContext _context;

        public CoursesController(FacultyWebAppContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index(string title, string programme, int semester)
        {
            IQueryable<Course> courses = _context.Course.AsQueryable();
            IQueryable<int> semesterQuery = _context.Course.OrderBy(m => m.Semester).Select(m => m.Semester).Distinct();
            IQueryable<string> programmesQuery = _context.Course.OrderBy(m => m.Programme).Select(m => m.Programme).Distinct();
            if (!string.IsNullOrEmpty(title))
            {
                courses = courses.Where(s => s.Title.Contains(title));
            }
            if (!string.IsNullOrEmpty(programme))
            {
                courses = courses.Where(c => c.Programme == programme);
            }

            if (semester != null && semester != 0)
            {
                courses = courses.Where(c => c.Semester == semester);
            }



            //  var FacultyContext = _context.Course.Include(m => m.Students)
            //                           .ThenInclude(m => m.student);


            var CourseTitleProgrammeVM = new CourseFilter
            {
                Courses = await courses.Include(c => c.Students).ThenInclude(c => c.student).ToListAsync(),
                Programmes = new SelectList(await programmesQuery.ToListAsync()),
                Semesters = new SelectList(await semesterQuery.ToListAsync()),

            };

            return View(CourseTitleProgrammeVM);
        }

        // GET: Courses/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["FirstTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id");
            ViewData["SecondTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Title,Credits,Semester,Programme,EducationLevel,FirstTeacherId,FirstTeacher,SecondTeacherId,SecondTeacher,Students")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Teachers"] = new SelectList(_context.Set<Teacher>(), "Id", "FullName");
            ViewData["Students"] = new SelectList(_context.Set<Student>(), "Id", "FullName");
            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = _context.Course.Where(c => c.Id == id).Include(c => c.Students).First();
            if (course == null)
                if (course == null)
            {
                return NotFound();
            }
            var students = _context.Student.AsEnumerable();
            students = students.OrderBy(s => s.FullName);


            EnrollStudents viewmodel = new EnrollStudents
            {
                Course = course,
                studentsEnrolledList = new MultiSelectList(students, "studentId", "FullName"),
                SelectedStudents = course.Students.Select(sa => sa.studentId),
            };

            ViewData["Teachers"] = new SelectList(_context.Set<Teacher>(), "Id", "FullName");
            return View(viewmodel);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, EnrollStudents viewmodel)
        {
            if (id != viewmodel.Course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewmodel.Course);
                    await _context.SaveChangesAsync();

                    var course = _context.Course.Where(m => m.Id == id).First();
                    string semester;

                    if (course.Semester%2 == 0)
                    {
                        semester = "Spring";
                    }
                    else
                    {
                        semester = "Autumn";
                    }

                    IEnumerable<long> listStudents = viewmodel.SelectedStudents;
                    IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(c => !listStudents.Contains(c.studentId) && c.CourseId == id);
                    _context.Enrollment.RemoveRange(toBeRemoved);

                    IEnumerable<long> existStudents = _context.Enrollment.Where(c => listStudents.Contains(c.studentId) && c.CourseId == id).Select(c => c.studentId);
                    IEnumerable<long> newStudents = listStudents.Where(c => !existStudents.Contains(c));
                    foreach (long StudentId in newStudents)
                        _context.Enrollment.Add(new Enrollment { studentId = StudentId, CourseId = id, Semester = semester });

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(viewmodel.Course.Id))
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

        // GET: Courses/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Course.FindAsync(id);
            _context.Course.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Admin, Teacher")]
        public async Task<IActionResult> Teaching(int? id, string title, int semester, string programme)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                           .FirstOrDefaultAsync(m => m.Id == id);
            IQueryable<Course> coursesQuery = _context.Course.Where(m => m.FirstTeacherId == id || m.SecondTeacherId == id);
            await _context.SaveChangesAsync();
            ViewBag.Message = teacher.FullName;
            if (teacher == null)
            {
                return NotFound();
            }
            IQueryable<int> semestersQuery = _context.Course.OrderBy(m => m.Semester).Select(m => m.Semester).Distinct();
            IQueryable<string> programmesQuery = _context.Course.OrderBy(m => m.Programme).Select(m => m.Programme).Distinct();
            if (!string.IsNullOrEmpty(title))
            {
                coursesQuery = coursesQuery.Where(x => x.Title.Contains(title));
            }
            if (semester != null && semester != 0)
            {
                coursesQuery = coursesQuery.Where(s => s.Semester == semester);
            }
            if (!string.IsNullOrEmpty(programme))
            {
                coursesQuery = coursesQuery.Where(p => p.Programme == programme);
            }

            var courseVM = new CourseViewModel
            {
                Courses = await coursesQuery.Include(c => c.FirstTeacher).Include(c => c.SecondTeacher).ToListAsync(),
                Programmes = new SelectList(await programmesQuery.ToListAsync()),
                Semester = new SelectList(await semestersQuery.ToListAsync())
            };

            return View(courseVM);
        }
    }

}
