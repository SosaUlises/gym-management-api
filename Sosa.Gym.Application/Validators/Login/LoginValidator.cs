using FluentValidation;
using Sosa.Gym.Application.DataBase.Login.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sosa.Gym.Application.Validators.Login
{
    public class LoginValidator : AbstractValidator<LoginModel>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);

        }
    }
}
