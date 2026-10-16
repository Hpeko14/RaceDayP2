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
    public class ResultsControllerTests
    {
        private RaceDayContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new RaceDayContext(options);
        }

        [Fact]
        public void Record_ShouldAddResult()
        {
            var context = GetDbContext();
            var user = new User { Name = "U", Email = "u@test.com", Password = "p" };
            var ev = new Event { Title = "E", Date = DateTime.Now, Location = "L" };
            context.Users.Add(user);
            context.Events.Add(ev);
            context.SaveChanges();

            var enrollment = new Enrollment { UserId = user.Id, EventId = ev.Id };
            context.Enrollments.Add(enrollment);
            context.SaveChanges();

            var controller = new ResultsController(context);
            var finish = TimeSpan.FromMinutes(30);
            var result = controller.Record(enrollment.Id, finish, 1) as OkObjectResult;

            var r = context.Results.FirstOrDefault(rr => rr.EnrollmentId == enrollment.Id);
            Assert.NotNull(r);
            Assert.Equal(finish, r.FinishTime);
            Assert.Equal(1, r.Position);
        }

        [Fact]
        public void GetByEvent_ShouldReturnResultsForEvent()
        {
            var context = GetDbContext();
            var ev = new Event { Title = "EventX", Date = DateTime.Now, Location = "L" };
            context.Events.Add(ev);
            context.SaveChanges();

            var en1 = new Enrollment { UserId = 1, EventId = ev.Id };
            var en2 = new Enrollment { UserId = 2, EventId = ev.Id };
            context.Enrollments.Add(en1);
            context.Enrollments.Add(en2);
            context.SaveChanges();

            context.Results.Add(new Result { EnrollmentId = en1.Id, FinishTime = TimeSpan.FromMinutes(20), Position = 1 });
            context.Results.Add(new Result { EnrollmentId = en2.Id, FinishTime = TimeSpan.FromMinutes(25), Position = 2 });
            context.SaveChanges();

            var controller = new ResultsController(context);
            var response = controller.GetByEvent(ev.Id) as OkObjectResult;
            var list = response?.Value as System.Collections.IEnumerable;
            Assert.NotNull(list);

            // Expect 2 results for the event
            Assert.Equal(2, context.Results.Where(r => new[] { en1.Id, en2.Id }.Contains(r.EnrollmentId)).Count());
        }
    }
}