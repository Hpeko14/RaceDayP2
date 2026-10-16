using Microsoft.AspNetCore.Http;
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

            // ✅ Record a result for an enrollment
            [HttpPost("record")]
            [RoleAuthorize("organiser")]
            public IActionResult Record(int enrollmentId, TimeSpan finishTime, int position)
            {
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

            // ✅ Get all results
            [HttpGet("all")]
            public IActionResult GetAll()
            {
                var results = _context.Results.ToList();
                return Ok(results);
            }

            // ✅ Get results for a specific event
            [HttpGet("event/{eventId}")]
            public IActionResult GetByEvent(int eventId)
            {
                var enrollments = _context.Enrollments
                    .Where(e => e.EventId == eventId)
                    .Select(e => e.Id)
                    .ToList();

                var results = _context.Results
                    .Where(r => enrollments.Contains(r.EnrollmentId))
                    .ToList();

                return Ok(results);
            }
        
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

        [HttpPut("update/{id}")]
        [RoleAuthorize("organiser")]
        public IActionResult Update(int id, TimeSpan finishTime, int position)
        {
            var result = _context.Results.Find(id);
            if (result == null) return NotFound();

            result.FinishTime = finishTime;
            result.Position = position;
            _context.SaveChanges();
            return Ok(new { message = "Result updated successfully", result });
        }

    }
}



