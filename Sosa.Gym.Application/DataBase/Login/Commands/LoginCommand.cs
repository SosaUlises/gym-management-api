using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Sosa.Gym.Application.External;
using Sosa.Gym.Application.Features;
using Sosa.Gym.Domain.Entidades.Usuario;
using Sosa.Gym.Domain.Models;

namespace Sosa.Gym.Application.DataBase.Login.Commands
{
    public class LoginCommand : ILoginCommand
    {
        private readonly UserManager<UsuarioEntity> _userManager;
        private readonly SignInManager<UsuarioEntity> _signInManager;
        private readonly IGetTokenJWTService _jwtService;

        public LoginCommand(
            UserManager<UsuarioEntity> userManager,
            SignInManager<UsuarioEntity> signInManager,
            IGetTokenJWTService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        public async Task<BaseResponseModel> Execute(LoginModel model)
        {

            var email = model.Email?.Trim();
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(model.Password))
            {
                return ResponseApiService.Response(StatusCodes.Status400BadRequest, "Email y contraseña son obligatorios");
            }


            var usuario = await _userManager.FindByEmailAsync(email);
            if (usuario == null)
                return ResponseApiService.Response(StatusCodes.Status401Unauthorized, "Usuario o contraseña incorrectos");

            var result = await _signInManager.CheckPasswordSignInAsync(usuario, model.Password, lockoutOnFailure: true);

            if (result.IsLockedOut)
                return ResponseApiService.Response(StatusCodes.Status423Locked, "Cuenta bloqueada temporalmente. Intenta más tarde.");

            if (!result.Succeeded)
                return ResponseApiService.Response(StatusCodes.Status401Unauthorized, "Usuario o contraseña incorrectos");

            var roles = await _userManager.GetRolesAsync(usuario);
            if (roles == null || roles.Count == 0)
                return ResponseApiService.Response(StatusCodes.Status403Forbidden, "Usuario sin rol asignado");

            var token = _jwtService.Execute(usuario.Id.ToString(), roles, usuario);

            return ResponseApiService.Response(StatusCodes.Status200OK, new
            {
                Token = token,
                Usuario = new
                {
                    usuario.Id,
                    usuario.Email,
                    usuario.Nombre,
                    usuario.Apellido,
                    Rol = roles
                }
            }, "Login exitoso");
        }

    }

}
