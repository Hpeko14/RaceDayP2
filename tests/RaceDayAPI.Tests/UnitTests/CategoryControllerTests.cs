using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Controllers;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using Xunit;

namespace RaceDayAPI.Tests
{
    public class CategoryControllerTests
    {
        private RaceDayContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new RaceDayContext(options);
        }

        [Fact]
        public void CreateCategory_ShouldAddCategory()
        {
            var context = GetDbContext();
            var controller = new CategoryController(context);

            var name = "Junior";
            var result = controller.Create(name) as OkObjectResult;

            var cat = context.Categories.FirstOrDefault(c => c.Name == name);
            Assert.NotNull(cat);
            Assert.Equal(name, cat.Name);
        }

        [Fact]
        public void GetAll_ShouldReturnCategories()
        {
            var context = GetDbContext();
            context.Categories.Add(new Category { Name = "C1" });
            context.Categories.Add(new Category { Name = "C2" });
            context.SaveChanges();

            var controller = new CategoryController(context);
            var result = controller.GetAll() as OkObjectResult;

            var list = result?.Value as System.Collections.IEnumerable;
            Assert.NotNull(list);
            Assert.Equal(2, context.Categories.Count());
        }
    }
}