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
        }// end of GetById method
        [HttpDelete("delete/{id}")]
        [RoleAuthorize("organiser")]
        public IActionResult Delete(int id)
        {// Delete an event by ID
            var ev = _context.Events.Find(id);
            if (ev == null) return NotFound();

            _context.Events.Remove(ev);
            _context.SaveChanges();
            return Ok(new { message = "Event deleted successfully" });
        }// end of Delete method
        [HttpPut("update/{id}")]
        [RoleAuthorize("organiser")]
        public IActionResult Update(int id, string title, DateTime date, string location)
        {// Update an existing event
            var ev = _context.Events.Find(id);
            if (ev == null) return NotFound();

            ev.Title = title;
            ev.Date = date;
            ev.Location = location;

            _context.SaveChanges();
            return Ok(new { message = "Event updated successfully", ev });
        }// end of Update method

    }// end of class
}// end of namespace



