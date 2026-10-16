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

            [HttpPost("enroll")]
            [RoleAuthorize("participant")]
            public IActionResult Enroll(int userId, int eventId)
            {
                // Ensure the caller is the same user or an organiser
                var sessionUserId = HttpContext.Session.GetInt32("UserId");
                var sessionRole = HttpContext.Session.GetString("Role");

                if (sessionUserId == null)
                    return Unauthorized();

                // Allow if the user is enrolling themselves or if the caller is an organiser
                if (sessionUserId.Value != userId && (sessionRole == null || sessionRole.Trim().ToLowerInvariant() != "organiser"))
                    return Forbid();

                var enrollment = new Enrollment { UserId = userId, EventId = eventId };
                _context.Enrollments.Add(enrollment);
                _context.SaveChanges();
                return Ok(enrollment);
            }

            [HttpGet("all")]
            public IActionResult GetAll()
            {
                var enrollments = _context.Enrollments.ToList();
                return Ok(enrollments);
            }
        [HttpDelete("delete/{id}")]
        [RoleAuthorize("participant")]
        public IActionResult Delete(int id)
        {
            var enrollment = _context.Enrollments.Find(id);
            if (enrollment == null) return NotFound();

            _context.Enrollments.Remove(enrollment);
            _context.SaveChanges();
            return Ok(new { message = "Enrollment cancelled successfully" });
        }
        [HttpPut("update/{id}")]
        [RoleAuthorize("participant")]
        public IActionResult Update(int id, int eventId)
        {
            var enrollment = _context.Enrollments.Find(id);
            if (enrollment == null) return NotFound();

            enrollment.EventId = eventId;
            _context.SaveChanges();
            return Ok(new { message = "Enrollment updated successfully", enrollment });
        }

    }
}



