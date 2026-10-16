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

        /// <summary>Creates a new category (organiser only).</summary>
        /// <param name="name">The category name.</param>
        [HttpPost("create")]
        [RoleAuthorize("organiser")]
        public IActionResult Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "Category name is required." });

            var category = new Category { Name = name.Trim() };
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

        /// <summary>Renames a category (organiser only).</summary>
        [HttpPut("update/{id}")]
        [RoleAuthorize("organiser")]
        public IActionResult Update(int id, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "Category name is required." });

            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();

            category.Name = name.Trim();
            _context.SaveChanges();
            return Ok(new { message = "Category updated successfully", category });
        }

        /// <summary>Deletes a category (organiser only).</summary>
        [HttpDelete("delete/{id}")]
        [RoleAuthorize("organiser")]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();

            _context.Categories.Remove(category);
            _context.SaveChanges();
            return Ok(new { message = "Category deleted successfully" });
        }
    }
}

