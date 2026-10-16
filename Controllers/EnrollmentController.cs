using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDayAPI.Data;
using RaceDayAPI.Helpers;
using RaceDayAPI.Models;



namespace RaceDayAPI.Controllers
{// start of namespace
    [ApiController]
        [Route("api/[controller]")]
        public class EnrollmentController : ControllerBase
    {// start of class
        private readonly RaceDayContext _context;

            public EnrollmentController(RaceDayContext context)
        {// start of constructor
            _context = context;
        }// end of constructor

        [HttpPost("enroll")]
            [RoleAuthorize("participant")]
            public IActionResult Enroll(int userId, int eventId)
        {// start of Enroll method
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
        }// end of Enroll method

        [HttpGet("all")]
            public IActionResult GetAll()
        {// start of GetAll method
            var enrollments = _context.Enrollments.ToList();
                return Ok(enrollments);
        }// end of GetAll method
        [HttpDelete("delete/{id}")]
        [RoleAuthorize("participant")]
        public IActionResult Delete(int id)
        {// start of Delete method
            var enrollment = _context.Enrollments.Find(id);
            if (enrollment == null) return NotFound();

            _context.Enrollments.Remove(enrollment);
            _context.SaveChanges();
            return Ok(new { message = "Enrollment cancelled successfully" });
        }// end of Delete method
        [HttpPut("update/{id}")]
        [RoleAuthorize("participant")]
        public IActionResult Update(int id, int eventId)
        {// start of Update method
            var enrollment = _context.Enrollments.Find(id);
            if (enrollment == null) return NotFound();

            enrollment.EventId = eventId;
            _context.SaveChanges();
            return Ok(new { message = "Enrollment updated successfully", enrollment });
        }// end of Update method

    }// end of class
}// end of namespace



