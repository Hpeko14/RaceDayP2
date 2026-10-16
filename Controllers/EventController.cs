using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDayAPI.Data;
using RaceDayAPI.Helpers;
using RaceDayAPI.Models;



namespace RaceDayAPI.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class EventController : ControllerBase
        {
            private readonly RaceDayContext _context;

            public EventController(RaceDayContext context)
            {
                _context = context;
            }

        // ✅ Create a new event
        [HttpPost("create")]
        [RoleAuthorize("organiser")]
        public IActionResult Create(string title, DateTime date, string location)
        {
            var ev = new Event { Title = title, Date = date, Location = location };
            _context.Events.Add(ev);
            _context.SaveChanges();
            return Ok(ev);
        }


        // ✅ Get all events
        [HttpGet("all")]
            public IActionResult GetAll()
            {
                var events = _context.Events.ToList();
                return Ok(events);
            }

            // ✅ Get event by ID
            [HttpGet("{id}")]
            public IActionResult GetById(int id)
            {
                var ev = _context.Events.Find(id);
                if (ev == null) return NotFound();
                return Ok(ev);
            }
        [HttpDelete("delete/{id}")]
        [RoleAuthorize("organiser")]
        public IActionResult Delete(int id)
        {
            var ev = _context.Events.Find(id);
            if (ev == null) return NotFound();

            _context.Events.Remove(ev);
            _context.SaveChanges();
            return Ok(new { message = "Event deleted successfully" });
        }
        [HttpPut("update/{id}")]
        [RoleAuthorize("organiser")]
        public IActionResult Update(int id, string title, DateTime date, string location)
        {
            var ev = _context.Events.Find(id);
            if (ev == null) return NotFound();

            ev.Title = title;
            ev.Date = date;
            ev.Location = location;

            _context.SaveChanges();
            return Ok(new { message = "Event updated successfully", ev });
        }

    }
}



