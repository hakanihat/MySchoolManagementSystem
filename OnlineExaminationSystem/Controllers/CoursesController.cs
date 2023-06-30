using System.Data.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineExaminationSystem.Data;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.ViewModels;

namespace OnlineExaminationSystem.Controllers
{
    [Authorize(Roles = "admin")]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SendGridEmailSender> _logger;


        public CoursesController(ApplicationDbContext context, ILogger<SendGridEmailSender> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            if (_context.Courses != null)
            {
                return View(await _context.Courses.ToListAsync());
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null || _context.Courses == null)
                {
                    return NotFound();
                }

                var course = await _context.Courses.FirstOrDefaultAsync(m => m.Id == id);
                if (course == null)
                {
                    return NotFound();
                }

                return View(course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving course details.");
                return RedirectToAction("Index", "Error");
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var course = new Course()
                    {
                        Name = viewModel.Name,
                        Description = viewModel.Description
                    };

                    _context.Add(course);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }

                return View(viewModel);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while creating a course.");
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a course.");
                return RedirectToAction("Index", "Error");
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null || _context.Courses == null)
                {
                    return NotFound();
                }

                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                {
                    return NotFound();
                }

                return View(course);
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "A database error occurred while retrieving the course for editing.");
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the course for editing.");
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
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
            return View(course);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null || _context.Courses == null)
                {
                    return NotFound();
                }

                var course = await _context.Courses.FirstOrDefaultAsync(m => m.Id == id);
                if (course == null)
                {
                    return NotFound();
                }

                return View(course);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting course.");
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the course id.");
                return RedirectToAction("Index", "Error");
            }
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (_context.Courses == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Courses' is null.");
                }

                var course = await _context.Courses.FindAsync(id);
                if (course != null)
                {
                    _context.Courses.Remove(course);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting course.");
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the course.");
                return RedirectToAction("Index", "Error");
            }
        }

        private bool CourseExists(int id)
        {
            try
            {
                return _context.Courses != null && _context.Courses.Any(e => e.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if the course exists.");
                return false;
            }
        }

    }
}
