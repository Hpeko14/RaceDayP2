using Microsoft.AspNetCore.Http;
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

            [HttpPost("create")]
            [RoleAuthorize("organiser")]
            public IActionResult Create(string name)
            {
                var category = new Category { Name = name };
                _context.Categories.Add(category);
                _context.SaveChanges();
                return Ok(category);
            }

            [HttpGet("all")]
            public IActionResult GetAll()
            {
                var categories = _context.Categories.ToList();
                return Ok(categories);
            }
        }
    }



