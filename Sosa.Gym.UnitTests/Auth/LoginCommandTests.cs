using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Sosa.Gym.Application.DataBase.Login.Commands;
using Sosa.Gym.Application.External;
using Sosa.Gym.Domain.Entidades.Usuario;
using Sosa.Gym.UnitTests.Helpers;
using Xunit;

namespace Sosa.Gym.UnitTests.Auth
{
    public class LoginCommandTests
    {
        [Fact]
        public async Task Execute_Returns400_WhenEmailOrPasswordMissing()
        {
            var um = IdentityMocks.MockUserManager();
            var sm = IdentityMocks.MockSignInManager(um.Object);
            var jwt = new Mock<IGetTokenJWTService>();

            var cmd = new LoginCommand(um.Object, sm.Object, jwt.Object);

            var res = await cmd.Execute(new LoginModel { Email = "", Password = "" });

            res.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            res.Success.Should().BeFalse();
        }

        [Fact]
        public async Task Execute_Returns401_WhenUserNotFound()
        {
            var um = IdentityMocks.MockUserManager();
            um.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((UsuarioEntity?)null);

            var sm = IdentityMocks.MockSignInManager(um.Object);
            var jwt = new Mock<IGetTokenJWTService>();

            var cmd = new LoginCommand(um.Object, sm.Object, jwt.Object);

            var res = await cmd.Execute(new LoginModel { Email = "a@a.com", Password = "123456" });

            res.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async Task Execute_Returns423_WhenLockedOut()
        {
            var user = new UsuarioEntity { Id = 10, Email = "a@a.com"};

            var um = IdentityMocks.MockUserManager();
            um.Setup(x => x.FindByEmailAsync("a@a.com")).ReturnsAsync(user);

            var sm = IdentityMocks.MockSignInManager(um.Object);
            sm.Setup(x => x.CheckPasswordSignInAsync(user, "123456", true))
              .ReturnsAsync(SignInResult.LockedOut);

            var jwt = new Mock<IGetTokenJWTService>();

            var cmd = new LoginCommand(um.Object, sm.Object, jwt.Object);

            var res = await cmd.Execute(new LoginModel { Email = "a@a.com", Password = "123456" });

            res.StatusCode.Should().Be(StatusCodes.Status423Locked);
        }

        [Fact]
        public async Task Execute_Returns200_WithToken_WhenOk()
        {
            var user = new UsuarioEntity { Id = 1, Email = "a@a.com", Nombre = "A", Apellido = "B" };

            var um = IdentityMocks.MockUserManager();
            um.Setup(x => x.FindByEmailAsync("a@a.com")).ReturnsAsync(user);
            um.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Administrador" });

            var sm = IdentityMocks.MockSignInManager(um.Object);
            sm.Setup(x => x.CheckPasswordSignInAsync(user, "123456", true))
              .ReturnsAsync(SignInResult.Success);

            var jwt = new Mock<IGetTokenJWTService>();
            jwt.Setup(x => x.Execute(user.Id.ToString(), It.IsAny<IEnumerable<string>>(), user))
               .Returns("fake-jwt");

            var cmd = new LoginCommand(um.Object, sm.Object, jwt.Object);

            var res = await cmd.Execute(new LoginModel { Email = "a@a.com", Password = "123456" });

            res.StatusCode.Should().Be(StatusCodes.Status200OK);
            res.Success.Should().BeTrue();
        }
    }
}
