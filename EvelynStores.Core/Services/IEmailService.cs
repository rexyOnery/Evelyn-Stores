namespace EvelynStores.Core.Services;

public interface IEmailService
{
    Task SendOtpEmailAsync(string toEmail, string otpCode);
}
