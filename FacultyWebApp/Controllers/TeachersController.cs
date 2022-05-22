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
    public class TeachersController : Controller
    {
        private readonly FacultyWebAppContext _context;

        public TeachersController(FacultyWebAppContext context)
        {
            _context = context;
        }

        // GET: Teachers
        public async Task<IActionResult> Index(string fullname, string degree, string academicRank)
        {
            IQueryable<Teacher> Teachers = _context.Teacher.AsQueryable();
            IQueryable<string> degreeQuery = _context.Teacher.OrderBy(m => m.Degree).Select(m => m.Degree).Distinct();
            IQueryable<string> academicRankQuery = _context.Teacher.OrderBy(m => m.AcademicRank).Select(m => m.AcademicRank).Distinct();
            if (!string.IsNullOrEmpty(fullname))
            {
                if (fullname.Contains(" "))
                {
                    string[] names = fullname.Split(" ");
                    Teachers = Teachers.Where(s => s.FirstName.Contains(names[0]) || s.LastName.Contains(names[1]));
                }
                else
                {
                    Teachers = Teachers.Where(s => s.FirstName.Contains(fullname) || s.LastName.Contains(fullname));
                }
            }

            if (!string.IsNullOrEmpty(academicRank))
            {
                Teachers = Teachers.Where(c => c.AcademicRank.Contains(academicRank));
            }

            if (!string.IsNullOrEmpty(degree))
            {
                Teachers = Teachers.Where(c => c.Degree.Contains(degree));
            }

            var TeachersFilter = new TeacherFilter
            {
                Teachers = await Teachers.Include(m => m.Courses1).Include(m => m.Courses2).ToListAsync(),
                Degrees = new SelectList(await degreeQuery.ToListAsync()),
                AcademicRanks = new SelectList(await academicRankQuery.ToListAsync()),

            };
            //            var mvcFacultyContext = _context.Teacher.Include(m => m.Courses1)
            //                                                  .Include(m => m.Courses2);
            return View(TeachersFilter);
        }

        // GET: Teachers/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            TeacherPicture teacherPicture = new TeacherPicture
            {
                teacher = teacher,
                profilePictureName = teacher.profilePicture,
            };

            return View(teacherPicture);
        }

        // GET: Teachers/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        // GET: Teachers/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate")] Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.Id))
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
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teacher.FindAsync(id);
            IQueryable<Course> course = _context.Course.AsQueryable();
            IQueryable<Course> courses1 = course.Where(x => x.FirstTeacherId == teacher.Id);
            IQueryable<Course> courses2 = course.Where(x => x.SecondTeacherId == teacher.Id);

            foreach (var courses in courses1)
            {
                courses.FirstTeacherId = null;
            }
            foreach (var courses in courses2)
            {
                courses.SecondTeacherId = null;
            }

            _context.Teacher.Remove(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherExists(int id)
        {
            return _context.Teacher.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditPicture(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = _context.Teacher.Where(x => x.Id == id).First();
            if (teacher == null)
            {
                return NotFound();
            }

            TeacherPicture viewmodel = new TeacherPicture
            {
                teacher = teacher,
                profilePictureName = teacher.profilePicture
            };

            return View(viewmodel);
        }

        // POST: Teachers/EditPicture/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditPicture(long id, TeacherPicture viewmodel)
        {
            if (id != viewmodel.teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (viewmodel.profilePictureFile != null)
                    {
                        string uniqueFileName = UploadedFile(viewmodel);
                        viewmodel.teacher.profilePicture = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.teacher.profilePicture = viewmodel.profilePictureName;
                    }

                    _context.Update(viewmodel.teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(viewmodel.teacher.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = viewmodel.teacher.Id });
            }
            return View(viewmodel);
        }

        private string UploadedFile(TeacherPicture viewmodel)
        {
            string uniqueFileName = null;

            if (viewmodel.profilePictureFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/profilePictures");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewmodel.profilePictureFile.FileName);
                string fileNameWithPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewmodel.profilePictureFile.CopyTo(stream);
                }
            }
            return uniqueFileName;
        }
    }
}