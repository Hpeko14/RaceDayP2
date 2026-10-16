using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDayAPI.Data;
using RaceDayAPI.Helpers;
using RaceDayAPI.Models;



namespace RaceDayAPI.Controllers
    {// start of namespace
        [ApiController]
        [Route("api/[controller]")]
        public class CategoryController : ControllerBase
        {// start of class
            private readonly RaceDayContext _context;

            public CategoryController(RaceDayContext context)
        {// start of constructor
            _context = context;
        }// end of constructor

        [HttpPost("create")]
            [RoleAuthorize("organiser")]
            public IActionResult Create(string name)
        {// start of Create method
            var category = new Category { Name = name };
                _context.Categories.Add(category);
                _context.SaveChanges();
                return Ok(category);
        }// end of Create method

        [HttpGet("all")]
            public IActionResult GetAll()
            {
                var categories = _context.Categories.ToList();
                return Ok(categories);
            }
    }// end of class
}// end of namespace



