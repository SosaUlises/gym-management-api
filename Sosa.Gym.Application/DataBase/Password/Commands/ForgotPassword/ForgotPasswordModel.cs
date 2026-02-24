namespace Sosa.Gym.Application.DataBase.Password.Commands.ForgotPassword
{
    public class ForgotPasswordModel
    {
        public string Email { get; set; } = default!;
        public string? FrontendResetUrl { get; set; }
    }
}
