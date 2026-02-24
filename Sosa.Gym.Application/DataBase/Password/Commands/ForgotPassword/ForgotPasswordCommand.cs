using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Sosa.Gym.Application.External;
using Sosa.Gym.Application.Features;
using Sosa.Gym.Domain.Entidades.Usuario;
using Sosa.Gym.Domain.Models;
using System.Net;

namespace Sosa.Gym.Application.DataBase.Password.Commands.ForgotPassword
{
    public class ForgotPasswordCommand : IForgotPasswordCommand
    {
        private readonly UserManager<UsuarioEntity> _userManager;
        private readonly IHostEnvironment _env;
        private readonly IEmailService _emailService;

        public ForgotPasswordCommand(
            UserManager<UsuarioEntity> userManager,
            IHostEnvironment env,
            IEmailService emailService)
        {
            _userManager = userManager;
            _env = env;
            _emailService = emailService;
        }

        public async Task<BaseResponseModel> Execute(ForgotPasswordModel model)
        {
            var email = model.Email?.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(email))
                return ResponseApiService.Response(StatusCodes.Status400BadRequest, "El email es obligatorio");

            // Respuesta siempre OK para no filtrar si existe o no
            const string genericMsg = "Si el email existe en el sistema, te enviaremos instrucciones para restablecer la contraseña.";

            if (string.IsNullOrWhiteSpace(model.FrontendResetUrl))
                return ResponseApiService.Response(StatusCodes.Status400BadRequest, "FrontendResetUrl es obligatorio");

            var usuario = await _userManager.FindByEmailAsync(email);

            if (usuario == null)
                return ResponseApiService.Response(StatusCodes.Status200OK, new { }, genericMsg);

            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
            var encodedToken = WebUtility.UrlEncode(token);

            var resetLink = $"{model.FrontendResetUrl}?email={WebUtility.UrlEncode(usuario.Email)}&token={encodedToken}";

            var subject = "Restablecer contraseña - Sosa GYM";
            var body = $@"
                <p>Hola {WebUtility.HtmlEncode(usuario.Nombre ?? usuario.Email ?? "usuario")},</p>
                <p>Recibimos una solicitud para restablecer tu contraseña.</p>
                <p>Hacé click en el siguiente enlace:</p>
                <p><a href=""{resetLink}"">Restablecer contraseña</a></p>
                <p>Si vos no solicitaste esto, podés ignorar este email.</p>
            ";

            try
            {
                await _emailService.SendAsync(usuario.Email!, subject, body);
            }
            catch
            {
                return ResponseApiService.Response(StatusCodes.Status500InternalServerError,
                    "No se pudo enviar el email. Intenta nuevamente más tarde.");
            }

            if (_env.IsDevelopment())
            {
                return ResponseApiService.Response(StatusCodes.Status200OK, new
                {
                    message = genericMsg,
                    resetLink
                }, "OK");
            }

            return ResponseApiService.Response(StatusCodes.Status200OK, new { }, genericMsg);
        }
    }
}
