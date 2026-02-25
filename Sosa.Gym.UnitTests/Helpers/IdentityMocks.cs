using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Sosa.Gym.Domain.Entidades.Usuario;

namespace Sosa.Gym.UnitTests.Helpers
{
    public static class IdentityMocks
    {
        public static Mock<UserManager<UsuarioEntity>> MockUserManager()
        {
            var store = new Mock<IUserStore<UsuarioEntity>>();
            return new Mock<UserManager<UsuarioEntity>>(
                store.Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<UsuarioEntity>>().Object,
                Array.Empty<IUserValidator<UsuarioEntity>>(),
                Array.Empty<IPasswordValidator<UsuarioEntity>>(),
                new Mock<ILookupNormalizer>().Object,
                new IdentityErrorDescriber(),
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<UsuarioEntity>>>().Object
            );
        }

        public static Mock<SignInManager<UsuarioEntity>> MockSignInManager(UserManager<UsuarioEntity> userManager)
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(a => a.HttpContext).Returns(new DefaultHttpContext());

            return new Mock<SignInManager<UsuarioEntity>>(
                userManager,
                contextAccessor.Object,
                new Mock<IUserClaimsPrincipalFactory<UsuarioEntity>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<UsuarioEntity>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<UsuarioEntity>>().Object
            );
        }
    }
}
