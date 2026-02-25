using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Moq;
using Sosa.Gym.Application.DataBase.Password.Commands.ForgotPassword;
using Sosa.Gym.Application.External;
using Sosa.Gym.Domain.Entidades.Usuario;
using Sosa.Gym.UnitTests.Helpers;
using Xunit;

namespace Sosa.Gym.UnitTests.Auth
{
    public class ForgotPasswordCommandTests
    {
        [Fact]
        public async Task Execute_Returns400_WhenEmailMissing()
        {
            var um = IdentityMocks.MockUserManager();
            var env = new Mock<IHostEnvironment>();
            var emailSvc = new Mock<IEmailService>();

            var cmd = new ForgotPasswordCommand(um.Object, env.Object, emailSvc.Object);

            var res = await cmd.Execute(new ForgotPasswordModel { Email = "", FrontendResetUrl = "https://x.com" });

            res.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Execute_Returns400_WhenFrontendResetUrlMissing()
        {
            var um = IdentityMocks.MockUserManager();
            var env = new Mock<IHostEnvironment>();
            var emailSvc = new Mock<IEmailService>();

            var cmd = new ForgotPasswordCommand(um.Object, env.Object, emailSvc.Object);

            var res = await cmd.Execute(new ForgotPasswordModel { Email = "a@a.com", FrontendResetUrl = "" });

            res.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Execute_Returns200_Generic_WhenUserNotFound()
        {
            var um = IdentityMocks.MockUserManager();
            um.Setup(x => x.FindByEmailAsync("a@a.com")).ReturnsAsync((UsuarioEntity?)null);

            var env = new Mock<IHostEnvironment>();
            env.SetupGet(e => e.EnvironmentName).Returns(Environments.Production);

            var emailSvc = new Mock<IEmailService>();

            var cmd = new ForgotPasswordCommand(um.Object, env.Object, emailSvc.Object);

            var res = await cmd.Execute(new ForgotPasswordModel
            {
                Email = "a@a.com",
                FrontendResetUrl = "https://x.com"
            });

            res.StatusCode.Should().Be(StatusCodes.Status200OK);
            res.Success.Should().BeTrue();

            emailSvc.Verify(x =>
                x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Execute_SendsEmail_WhenUserExistsAndActive()
        {
            var user = new UsuarioEntity { Email = "a@a.com", Nombre = "A" };

            var um = IdentityMocks.MockUserManager();
            um.Setup(x => x.FindByEmailAsync("a@a.com")).ReturnsAsync(user);
            um.Setup(x => x.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("token++");

            var env = new Mock<IHostEnvironment>();
            env.SetupGet(e => e.EnvironmentName).Returns(Environments.Production);

            var emailSvc = new Mock<IEmailService>();
            emailSvc.Setup(x => x.SendAsync(
                    "a@a.com",
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var cmd = new ForgotPasswordCommand(um.Object, env.Object, emailSvc.Object);

            var res = await cmd.Execute(new ForgotPasswordModel
            {
                Email = "a@a.com",
                FrontendResetUrl = "https://front/reset"
            });

            res.StatusCode.Should().Be(StatusCodes.Status200OK);

            emailSvc.Verify(x => x.SendAsync(
                    "a@a.com",
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        }
    }
}
