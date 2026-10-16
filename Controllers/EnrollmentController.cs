using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDayAPI.Data;
using RaceDayAPI.Helpers;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public EnrollmentController(RaceDayContext context)
        {
            _context = context;
        }

        private bool IsOrganiser()
        {
            var role = HttpContext.Session.GetString("Role");
            return role != null && role.Trim().ToLowerInvariant() == "organiser";
        }

        /// <summary>Enrols a user in a category. Participants can only enrol themselves.</summary>
        [HttpPost("enroll")]
        [RoleAuthorize("participant")]
        public IActionResult Enroll(int userId, int categoryId)
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId == null) return Unauthorized();

            if (sessionUserId.Value != userId && !IsOrganiser())
                return Forbid();

            if (!_context.Categories.Any(c => c.Id == categoryId))
                return NotFound(new { message = "Category not found." });

            if (_context.Enrollments.Any(e => e.UserId == userId && e.CategoryId == categoryId))
                return Conflict(new { message = "User is already enrolled in this category." });

            var enrollment = new Enrollment
            {
                UserId = userId,
                CategoryId = categoryId,
                RegistrationDate = DateTime.Today
            };

            _context.Enrollments.Add(enrollment);
            _context.SaveChanges();
            return Ok(enrollment);
        }

        /// <summary>Returns all enrollments.</summary>
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            return Ok(_context.Enrollments.ToList());
        }

        /// <summary>Cancels an enrollment. Only the owner or an organiser may do this.</summary>
        [HttpDelete("delete/{id}")]
        [RoleAuthorize("participant")]
        public IActionResult Delete(int id)
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId == null) return Unauthorized();

            var enrollment = _context.Enrollments.Find(id);
            if (enrollment == null) return NotFound();

            if (enrollment.UserId != sessionUserId.Value && !IsOrganiser())
                return Forbid();

            if (_context.Results.Any(r => r.EnrollmentId == id))
                return Conflict(new { message = "Cannot cancel an enrollment that already has a result." });

            _context.Enrollments.Remove(enrollment);
            _context.SaveChanges();
            return Ok(new { message = "Enrollment cancelled successfully" });
        }

        /// <summary>Moves an enrollment to another category. Only the owner or an organiser may do this.</summary>
        [HttpPut("update/{id}")]
        [RoleAuthorize("participant")]
        public IActionResult Update(int id, int categoryId)
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId == null) return Unauthorized();

            var enrollment = _context.Enrollments.Find(id);
            if (enrollment == null) return NotFound();

            if (enrollment.UserId != sessionUserId.Value && !IsOrganiser())
                return Forbid();

            if (!_context.Categories.Any(c => c.Id == categoryId))
                return NotFound(new { message = "Category not found." });

            if (_context.Enrollments.Any(e => e.UserId == enrollment.UserId
                                              && e.CategoryId == categoryId && e.Id != id))
                return Conflict(new { message = "User is already enrolled in that category." });

            enrollment.CategoryId = categoryId;
            _context.SaveChanges();
            return Ok(new { message = "Enrollment updated successfully", enrollment });
        }
    }
}