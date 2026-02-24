using Sosa.Gym.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sosa.Gym.Application.DataBase.Password.Commands.ResetPassword
{
    public interface IResetPasswordCommand
    {
        Task<BaseResponseModel> Execute(ResetPasswordModel model);
    }
}
