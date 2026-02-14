using System.Net;
using System.Net.Mail;
using EvelynStores.Core.Models;
using EvelynStores.Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EvelynStores.Infrastructure.Services;

public class EmailService(IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger) : IEmailService
{
    private readonly SmtpSettings _smtpSettings = smtpSettings.Value;

    public async Task SendOtpEmailAsync(string toEmail, string otpCode)
    {
        try
        {
            using var message = new MailMessage();
            message.From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = "Password Reset OTP - EvePhil Supermarket";
            message.IsBodyHtml = true;
            message.Body = $"""
                <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
                    <h2 style="color: #333;">Password Reset Request</h2>
                    <p>You have requested to reset your password. Use the OTP code below to verify your identity:</p>
                    <div style="background-color: #f4f4f4; padding: 20px; text-align: center; margin: 20px 0; border-radius: 8px;">
                        <h1 style="color: #e74c3c; letter-spacing: 10px; margin: 0;">{otpCode}</h1>
                    </div>
                    <p>This code will expire in <strong>10 minutes</strong>.</p>
                    <p>If you did not request this, please ignore this email.</p>
                    <hr style="border: none; border-top: 1px solid #eee; margin: 20px 0;" />
                    <p style="color: #999; font-size: 12px;">EvePhil Supermarket</p>
                </div>
                """;

            using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port);
            client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
            client.EnableSsl = _smtpSettings.EnableSsl;

            await client.SendMailAsync(message);

            logger.LogInformation("OTP email sent to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send OTP email to {Email}", toEmail);
            throw;
        }
    }
}
