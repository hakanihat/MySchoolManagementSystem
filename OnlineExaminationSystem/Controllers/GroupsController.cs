using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineExaminationSystem.Data;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem.Controllers
{
    [Authorize(Roles = "admin")]
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SendGridEmailSender> _logger;

        public GroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Groups
        public async Task<IActionResult> Index()
        {
            if (_context.Groups != null)
            {
                return View(await _context.Groups.ToListAsync());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        // GET: Groups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null || _context.Groups == null)
                {
                    return NotFound();
                }

                var @group = await _context.Groups
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (@group == null)
                {
                    return NotFound();
                }

                return View(@group);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while retrieving group details.");

                // Redirect to the desired page
                return RedirectToAction("Index", "Home");
            }
        }


        // GET: Groups/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Group @group)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(@group);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                return View(@group);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while creating a group.");

                // Redirect to an error page or display an error message
                return RedirectToAction("Error", "Home");
            }
        }


        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null || _context.Groups == null)
                {
                    return NotFound();
                }

                var @group = await _context.Groups.FindAsync(id);
                if (@group == null)
                {
                    return NotFound();
                }

                return View(@group);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving group for editing.");
                return RedirectToAction("Error", "Home");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Group @group)
        {
            try
            {
                if (id != @group.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(@group);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!GroupExists(@group.Id))
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
                return View(@group);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while updating the group.");

                // Redirect to an error page or display an error message
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null || _context.Groups == null)
                {
                    return NotFound();
                }

                var @group = await _context.Groups
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (@group == null)
                {
                    return NotFound();
                }

                return View(@group);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while retrieving the group for deletion.");

                // Redirect to an error page or display an error message
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (_context.Groups == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Groups' is null.");
                }

                var @group = await _context.Groups.FindAsync(id);
                if (@group != null)
                {
                    _context.Groups.Remove(@group);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while deleting the group.");

                // Redirect to an error page or display an error message
                return RedirectToAction("Error", "Home");
            }
        }


        private bool GroupExists(int id)
        {
            try
            {
                return (_context.Groups?.Any(e => e.Id == id)).GetValueOrDefault();
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while checking if the group exists.");

                // Return an appropriate default value or handle the exception as needed
                return false;
            }
        }

    }
}
