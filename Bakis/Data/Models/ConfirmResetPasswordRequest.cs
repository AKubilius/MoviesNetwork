namespace Bakis.Data.Models
{
    public class ConfirmResetPasswordRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string Email { get; set; }
    }
}
