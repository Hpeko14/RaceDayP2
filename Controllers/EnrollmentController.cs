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

        /// <summary>Enrols a user in an event. Participants can only enrol themselves.</summary>
        [HttpPost("enroll")]
        [RoleAuthorize("participant")]
        public IActionResult Enroll(int userId, int eventId)
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId == null) return Unauthorized();

            if (sessionUserId.Value != userId && !IsOrganiser())
                return Forbid();

            if (!_context.Events.Any(e => e.Id == eventId))
                return NotFound(new { message = "Event not found." });

            if (_context.Enrollments.Any(e => e.UserId == userId && e.EventId == eventId))
                return Conflict(new { message = "User is already enrolled in this event." });

            var enrollment = new Enrollment { UserId = userId, EventId = eventId };
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

            _context.Enrollments.Remove(enrollment);
            _context.SaveChanges();
            return Ok(new { message = "Enrollment cancelled successfully" });
        }

        /// <summary>Moves an enrollment to another event. Only the owner or an organiser may do this.</summary>
        [HttpPut("update/{id}")]
        [RoleAuthorize("participant")]
        public IActionResult Update(int id, int eventId)
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId == null) return Unauthorized();

            var enrollment = _context.Enrollments.Find(id);
            if (enrollment == null) return NotFound();

            if (enrollment.UserId != sessionUserId.Value && !IsOrganiser())
                return Forbid();

            if (!_context.Events.Any(e => e.Id == eventId))
                return NotFound(new { message = "Event not found." });

            if (_context.Enrollments.Any(e => e.UserId == enrollment.UserId
                                              && e.EventId == eventId && e.Id != id))
                return Conflict(new { message = "User is already enrolled in that event." });

            enrollment.EventId = eventId;
            _context.SaveChanges();
            return Ok(new { message = "Enrollment updated successfully", enrollment });
        }
    }
}
