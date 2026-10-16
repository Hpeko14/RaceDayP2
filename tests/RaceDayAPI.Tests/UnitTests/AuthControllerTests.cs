using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Controllers;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using Xunit;

namespace RaceDayAPI.Tests
{
    public class AuthControllerTests
    {
        private RaceDayContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            return new RaceDayContext(options);
        }

        [Fact]
        public void Register_ShouldHashPassword()
        {
            var context = GetDbContext();
            var controller = new AuthController(context);

            var result = controller.Register("TestUser", "test@mail.com", "password123", "organiser") as OkObjectResult;

            var user = context.Users.FirstOrDefault(u => u.Email == "test@mail.com");
            Assert.NotNull(user);
            Assert.NotEqual("password123", user.Password); // password should be hashed
        }
    }
}