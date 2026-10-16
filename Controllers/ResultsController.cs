using Microsoft.AspNetCore.Mvc;
using RaceDayAPI.Data;
using RaceDayAPI.Helpers;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public ResultsController(RaceDayContext context)
        {
            _context = context;
        }

        /// <summary>Records a result for an enrollment (organiser only).</summary>
        [HttpPost("record")]
        [RoleAuthorize("organiser")]
        public IActionResult Record(int enrollmentId, TimeSpan finishTime, int position)
        {
            if (position < 1)
                return BadRequest(new { message = "Position must be 1 or greater." });

            if (!_context.Enrollments.Any(e => e.Id == enrollmentId))
                return NotFound(new { message = "Enrollment not found." });

            if (_context.Results.Any(r => r.EnrollmentId == enrollmentId))
                return Conflict(new { message = "A result already exists for this enrollment." });

            var result = new Result
            {
                EnrollmentId = enrollmentId,
                FinishTime = finishTime,
                Position = position
            };

            _context.Results.Add(result);
            _context.SaveChanges();
            return Ok(result);
        }

        /// <summary>Returns all results.</summary>
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            return Ok(_context.Results.ToList());
        }

        /// <summary>Returns all results for a specific event.</summary>
        [HttpGet("event/{eventId}")]
        public IActionResult GetByEvent(int eventId)
        {
            var results = _context.Results
                .Where(r => _context.Enrollments
                    .Any(e => e.Id == r.EnrollmentId && e.EventId == eventId))
                .OrderBy(r => r.Position)
                .ToList();

            return Ok(results);
        }

        /// <summary>Updates a result (organiser only).</summary>
        [HttpPut("update/{id}")]
        [RoleAuthorize("organiser")]
        public IActionResult Update(int id, TimeSpan finishTime, int position)
        {
            if (position < 1)
                return BadRequest(new { message = "Position must be 1 or greater." });

            var result = _context.Results.Find(id);
            if (result == null) return NotFound();

            result.FinishTime = finishTime;
            result.Position = position;
            _context.SaveChanges();
            return Ok(new { message = "Result updated successfully", result });
        }

        /// <summary>Deletes a result (organiser only).</summary>
        [HttpDelete("delete/{id}")]
        [RoleAuthorize("organiser")]
        public IActionResult Delete(int id)
        {
            var result = _context.Results.Find(id);
            if (result == null) return NotFound();

            _context.Results.Remove(result);
            _context.SaveChanges();
            return Ok(new { message = "Result deleted successfully" });
        }
    }
}
