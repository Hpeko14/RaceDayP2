using Microsoft.AspNetCore.Mvc;
using RaceDayAPI.Data;
using RaceDayAPI.Helpers;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public CategoryController(RaceDayContext context)
        {
            _context = context;
        }

        /// <summary>Creates a category for an event (organiser only).</summary>
        [HttpPost("create")]
        [RoleAuthorize("organiser")]
        public IActionResult Create(int eventId, string name, decimal distance)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "Category name is required." });

            if (distance <= 0)
                return BadRequest(new { message = "Distance must be greater than 0." });

            if (!_context.Events.Any(e => e.Id == eventId))
                return NotFound(new { message = "Event not found." });

            var category = new Category { EventId = eventId, Name = name.Trim(), Distance = distance };
            _context.Categories.Add(category);
            _context.SaveChanges();
            return Ok(category);
        }

        /// <summary>Returns all categories.</summary>
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            return Ok(_context.Categories.ToList());
        }

        /// <summary>Updates a category (organiser only).</summary>
        [HttpPut("update/{id}")]
        [RoleAuthorize("organiser")]
        public IActionResult Update(int id, int eventId, string name, decimal distance)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "Category name is required." });

            if (distance <= 0)
                return BadRequest(new { message = "Distance must be greater than 0." });

            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();

            if (!_context.Events.Any(e => e.Id == eventId))
                return NotFound(new { message = "Event not found." });

            category.EventId = eventId;
            category.Name = name.Trim();
            category.Distance = distance;
            _context.SaveChanges();
            return Ok(new { message = "Category updated successfully", category });
        }

        /// <summary>Deletes a category (organiser only). Fails if people are enrolled in it.</summary>
        [HttpDelete("delete/{id}")]
        [RoleAuthorize("organiser")]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();

            if (_context.Enrollments.Any(e => e.CategoryId == id))
                return Conflict(new { message = "Cannot delete a category that has enrollments." });

            _context.Categories.Remove(category);
            _context.SaveChanges();
            return Ok(new { message = "Category deleted successfully" });
        }
    }
}