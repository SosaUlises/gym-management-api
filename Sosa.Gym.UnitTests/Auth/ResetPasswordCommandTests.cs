using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Sosa.Gym.Application.DataBase.Password.Commands.ResetPassword;
using Sosa.Gym.Domain.Entidades.Usuario;
using Sosa.Gym.UnitTests.Helpers;
using Xunit;

namespace Sosa.Gym.UnitTests.Auth
{
    public class ResetPasswordCommandTests
    {
        [Fact]
        public async Task Execute_Returns400_WhenPasswordsDontMatch()
        {
            var um = IdentityMocks.MockUserManager();
            var cmd = new ResetPasswordCommand(um.Object);

            var res = await cmd.Execute(new ResetPasswordModel
            {
                Email = "a@a.com",
                Token = "t",
                NewPassword = "123456",
                ConfirmPassword = "999999"
            });

            res.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Execute_Returns200_WhenResetOk()
        {
            var user = new UsuarioEntity { Email = "a@a.com"};

            var um = IdentityMocks.MockUserManager();
            um.Setup(x => x.FindByEmailAsync("a@a.com")).ReturnsAsync(user);
            um.Setup(x => x.ResetPasswordAsync(user, "t", "123456")).ReturnsAsync(IdentityResult.Success);

            var cmd = new ResetPasswordCommand(um.Object);

            var res = await cmd.Execute(new ResetPasswordModel
            {
                Email = "a@a.com",
                Token = "t",
                NewPassword = "123456",
                ConfirmPassword = "123456"
            });

            res.StatusCode.Should().Be(StatusCodes.Status200OK);
            res.Success.Should().BeTrue();
        }
    }
}
