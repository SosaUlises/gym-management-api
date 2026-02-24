using FluentValidation;
using Sosa.Gym.Application.DataBase.Password.Commands.ForgotPassword;

namespace Sosa.Gym.Application.Validators.Password
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordModel>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("El email es obligatorio.")
                    .EmailAddress().WithMessage("El formato del email no es válido.");

        }
    }
}
