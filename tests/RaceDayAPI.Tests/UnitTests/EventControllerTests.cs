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
    public class EventControllerTests
    {
        private RaceDayContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new RaceDayContext(options);
        }

        [Fact]
        public void CreateEvent_ShouldAddEvent()
        {
            var context = GetDbContext();
            var controller = new EventController(context);

            var title = "Test Event";
            var date = new DateTime(2026, 1, 1);
            var location = "Test Location";

            var result = controller.Create(title, date, location) as OkObjectResult;

            var ev = context.Events.FirstOrDefault(e => e.Title == title);
            Assert.NotNull(ev);
            Assert.Equal(location, ev.Location);
            Assert.Equal(date, ev.Date);
        }

        [Fact]
        public void GetAll_ShouldReturnAllEvents()
        {
            var context = GetDbContext();
            context.Events.Add(new Event { Title = "A", Date = DateTime.Now, Location = "L1" });
            context.Events.Add(new Event { Title = "B", Date = DateTime.Now, Location = "L2" });
            context.SaveChanges();

            var controller = new EventController(context);
            var result = controller.GetAll() as OkObjectResult;

            var list = result?.Value as System.Collections.IEnumerable;
            Assert.NotNull(list);
            Assert.Equal(2, context.Events.Count());
        }
    }
}