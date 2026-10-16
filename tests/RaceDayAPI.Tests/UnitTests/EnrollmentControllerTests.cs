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
    public class EnrollmentControllerTests
    {
        private RaceDayContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new RaceDayContext(options);
        }

        [Fact]
        public void Enroll_ShouldAddEnrollment()
        {
            var context = GetDbContext();

            var user = new User { Name = "U1", Email = "u1@test.com", Password = "x" };
            var ev = new Event { Title = "E1", Date = DateTime.Now, Location = "L" };
            context.Users.Add(user);
            context.Events.Add(ev);
            context.SaveChanges();

            var controller = new EnrollmentController(context);
            var result = controller.Enroll(user.Id, ev.Id) as OkObjectResult;

            var enrollment = context.Enrollments.FirstOrDefault(en => en.UserId == user.Id && en.EventId == ev.Id);
            Assert.NotNull(enrollment);
        }

        [Fact]
        public void GetAll_ShouldReturnEnrollments()
        {
            var context = GetDbContext();
            context.Enrollments.Add(new Enrollment { UserId = 1, EventId = 2 });
            context.Enrollments.Add(new Enrollment { UserId = 2, EventId = 3 });
            context.SaveChanges();

            var controller = new EnrollmentController(context);
            var result = controller.GetAll() as OkObjectResult;

            var list = result?.Value as System.Collections.IEnumerable;
            Assert.NotNull(list);
            Assert.Equal(2, context.Enrollments.Count());
        }
    }
}