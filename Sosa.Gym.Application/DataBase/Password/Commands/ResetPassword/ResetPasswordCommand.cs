using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Sosa.Gym.Application.Features;
using Sosa.Gym.Domain.Entidades.Usuario;
using Sosa.Gym.Domain.Models;
using System.Net;

namespace Sosa.Gym.Application.DataBase.Password.Commands.ResetPassword
{
    public class ResetPasswordCommand : IResetPasswordCommand
    {
        private readonly UserManager<UsuarioEntity> _userManager;

        public ResetPasswordCommand(UserManager<UsuarioEntity> userManager)
        {
            _userManager = userManager;
        }

        public async Task<BaseResponseModel> Execute(ResetPasswordModel model)
        {
            var email = model.Email?.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(model.Token) ||
                string.IsNullOrWhiteSpace(model.NewPassword) ||
                string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                return ResponseApiService.Response(StatusCodes.Status400BadRequest, "Email, token y contraseñas son obligatorios");
            }

            if (model.NewPassword != model.ConfirmPassword)
                return ResponseApiService.Response(StatusCodes.Status400BadRequest, "Las contraseñas no coinciden");

            var usuario = await _userManager.FindByEmailAsync(email);
            if (usuario == null)
            {
                return ResponseApiService.Response(StatusCodes.Status400BadRequest, "Token inválido o expirado");
            }


            var decodedToken = WebUtility.UrlDecode(model.Token)?.Trim();
            if (string.IsNullOrWhiteSpace(decodedToken))
                return ResponseApiService.Response(StatusCodes.Status400BadRequest, "Token inválido o expirado");


            var result = await _userManager.ResetPasswordAsync(usuario, decodedToken, model.NewPassword);

            if (!result.Succeeded)
            {
                return ResponseApiService.Response(StatusCodes.Status400BadRequest,
                    result.Errors.Select(e => e.Description).ToList(),
                    "No se pudo restablecer la contraseña");
            }

            return ResponseApiService.Response(StatusCodes.Status200OK, new { }, "Contraseña restablecida correctamente");
        }
    }
}
