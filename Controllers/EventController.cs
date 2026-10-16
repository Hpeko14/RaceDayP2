using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDayAPI.Data;
using RaceDayAPI.Helpers;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{//Start of namespace
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {//Start of class
        private readonly RaceDayContext _context;

        public EventController(RaceDayContext context)
        {
            _context = context;
        }

        /// <summary>Creates an event owned by the logged-in organiser.</summary>
        [HttpPost("create")]
        [RoleAuthorize("organiser")]
        public IActionResult Create(string title, DateTime date, string location)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(location))
                return BadRequest(new { message = "Title and location are required." });

            var organiserId = HttpContext.Session.GetInt32("UserId");
            if (organiserId == null) return Unauthorized();

            var ev = new Event
            {
                Title = title.Trim(),
                Date = date,
                Location = location.Trim(),
                OrganiserId = organiserId.Value
            };

            _context.Events.Add(ev);
            _context.SaveChanges();
            return Ok(ev);
        }

        /// <summary>Returns all events.</summary>
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            return Ok(_context.Events.ToList());
        }

        /// <summary>Returns a single event by ID.</summary>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var ev = _context.Events.Find(id);
            if (ev == null) return NotFound();
            return Ok(ev);
        }

        /// <summary>Updates an event (organiser only).</summary>
        [HttpPut("update/{id}")]
        [RoleAuthorize("organiser")]
        public IActionResult Update(int id, string title, DateTime date, string location)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(location))
                return BadRequest(new { message = "Title and location are required." });

            var ev = _context.Events.Find(id);
            if (ev == null) return NotFound();

            ev.Title = title.Trim();
            ev.Date = date;
            ev.Location = location.Trim();
            _context.SaveChanges();
            return Ok(new { message = "Event updated successfully", ev });
        }

        /// <summary>Deletes an event (organiser only). Fails if it still has categories or routes.</summary>
        [HttpDelete("delete/{id}")]
        [RoleAuthorize("organiser")]
        public IActionResult Delete(int id)
        {
            var ev = _context.Events.Find(id);
            if (ev == null) return NotFound();

            if (_context.Categories.Any(c => c.EventId == id) || _context.Routes.Any(r => r.EventId == id))
                return Conflict(new { message = "Cannot delete an event that has categories or routes." });

            _context.Events.Remove(ev);
            _context.SaveChanges();
            return Ok(new { message = "Event deleted successfully" });
        }
    }//End of class
}//End of namespace