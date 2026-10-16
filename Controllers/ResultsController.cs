using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDayAPI.Data;
using RaceDayAPI.Helpers;
using RaceDayAPI.Models;



namespace RaceDayAPI.Controllers
{// start of namespace
    [ApiController]
        [Route("api/[controller]")]
        public class ResultsController : ControllerBase
    {// start of class
        private readonly RaceDayContext _context;

            public ResultsController(RaceDayContext context)
        {// start of constructor
            _context = context;
        }// end of constructor

        // ✅ Record a result for an enrollment
        [HttpPost("record")]
            [RoleAuthorize("organiser")]
            public IActionResult Record(int enrollmentId, TimeSpan finishTime, int position)
        {// start of Record method
            var result = new Result
            {// start of object initializer
                EnrollmentId = enrollmentId,
                    FinishTime = finishTime,
                    Position = position
                };// end of object initializer

            _context.Results.Add(result);
                _context.SaveChanges();
                return Ok(result);
        }// end of Record method

        // ✅ Get all results
        [HttpGet("all")]
            public IActionResult GetAll()
        {// start of GetAll method
            var results = _context.Results.ToList();
                return Ok(results);
        }// end of GetAll method

        // ✅ Get results for a specific event
        [HttpGet("event/{eventId}")]
            public IActionResult GetByEvent(int eventId)
        {// start of GetByEvent method
            var enrollments = _context.Enrollments
                    .Where(e => e.EventId == eventId)
                    .Select(e => e.Id)
                    .ToList();

                var results = _context.Results
                    .Where(r => enrollments.Contains(r.EnrollmentId))
                    .ToList();

                return Ok(results);
        }// end of GetByEvent method

        [HttpDelete("delete/{id}")]
        [RoleAuthorize("organiser")]
        public IActionResult Delete(int id)
        {// start of Delete method
            var result = _context.Results.Find(id);
            if (result == null) 
                
                return NotFound();

            _context.Results.Remove(result);
            _context.SaveChanges();
            return Ok(new { message = "Result deleted successfully" });
        }// end of Delete method

        [HttpPut("update/{id}")]
        [RoleAuthorize("organiser")]
        public IActionResult Update(int id, TimeSpan finishTime, int position)
        {// start of Update method
            var result = _context.Results.Find(id);
            if (result == null) 
                
                return NotFound();

            result.FinishTime = finishTime;
            result.Position = position;
            _context.SaveChanges();
            return Ok(new { message = "Result updated successfully", result });
        }// end of Update method

    }// end of class
}// end of namespace



