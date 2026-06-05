using System.Net;
using System.Net.Mail;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var smtpServer = smtpSettings["Server"];
                var smtpPort = int.Parse(smtpSettings["Port"] ?? "587");
                var senderEmail = smtpSettings["SenderEmail"];
                var senderPassword = smtpSettings["SenderPassword"];
                var enableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true");

                if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(senderEmail))
                {
                    _logger.LogWarning("SMTP settings not configured. Email not sent to {Email}", toEmail);
                    return false;
                }

                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = enableSsl,
                    Timeout = 10000
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendOtpEmailAsync(string toEmail, string otp, string userName)
        {
            try
            {
                var subject = "Your OTP Code - Help Empowerment";
                var body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #333;'>Password Reset OTP</h2>
                            <p>Hello {userName},</p>
                            <p>You requested a password reset. Please use the following One-Time Password (OTP) to proceed:</p>
                            <div style='background-color: #f0f0f0; padding: 15px; border-radius: 5px; text-align: center;'>
                                <h3 style='color: #007bff; font-size: 32px; letter-spacing: 5px;'>{otp}</h3>
                            </div>
                            <p style='color: #666; font-size: 14px;'>
                                <strong>Note:</strong> This OTP is valid for 10 minutes only.
                            </p>
                            <p style='color: #666; font-size: 14px;'>
                                If you did not request a password reset, please ignore this email.
                            </p>
                            <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;'>
                            <p style='color: #999; font-size: 12px; text-align: center;'>
                                &copy; 2024 Help Empowerment. All rights reserved.
                            </p>
                        </div>
                    </body>
                    </html>";

                return await SendEmailAsync(toEmail, subject, body, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending OTP email to {Email}", toEmail);
                return false;
            }
        }
    }
}
