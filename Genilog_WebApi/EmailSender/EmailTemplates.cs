using System.Net.Http.Json;

namespace Genilog_WebApi.EmailSender
{
    public class EmailTemplates
    {
        private static readonly HttpClient _http = new HttpClient();

        private static string ResendApiKey => Environment.GetEnvironmentVariable("ResendApiKey") ?? "";
        private static string FromAddress => Environment.GetEnvironmentVariable("MailSettings__Mail") ?? "noreply@ginilog.com";
        private static string FromName => "Ginilog";

        private static async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");
            request.Headers.Add("Authorization", $"Bearer {ResendApiKey}");
            request.Content = JsonContent.Create(new
            {
                from = $"{FromName} <{FromAddress}>",
                to = new[] { toEmail },
                subject,
                html = htmlBody
            });

            var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public static async Task SendEmailVerificationCode(string emailId, string activationcode, string username)
        {
            string subject = "Verify Your Email Address";

            string body = $@"
             <html>
             <body style='font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; text-align: center;'>
             <div style='max-width: 600px; margin: auto; background: white; padding: 20px; border-radius: 5px;'>

            <h2 style='color: #0046BE; margin: 0;'>Ginilog Ltd</h2>
            <hr style='border: 1px solid #ddd;'>

            <h2 style='color: #0046BE;'>Verify Your Email Address</h2>
            <p style='font-size: 16px;'>Hi {username},</p>
            <p style='color: #666;'>You're almost ready to get started. Please use the code below to verify your account.</p>

            <div style='background-color: #ff6600; color: white; padding: 12px 20px; font-size: 16px;
                        border-radius: 5px; display: inline-block; font-weight: bold;'>
                {activationcode}
            </div>

            <br/><br/>
            <p style='font-size: 14px; color: #888;'>This code expires in 10 minutes.</p>

            <br/><br/>
            <p style='font-size: 1.18rem;'>Thanks, <br> Ginilog Team</p>

            <br/><br/>
            <div style='border-top: 1px solid #ddd; padding-top: 10px; text-align: left;'>
                <p style='color: #666; font-weight: bold;'>Follow us on:</p>
                <p><a href='https://www.facebook.com/profile.php?id=61565116785067' style='color: #0046BE; text-decoration: none;'>Facebook</a></p>
                <p><a href='https://twitter.com/' style='color: #0046BE; text-decoration: none;'>Twitter</a></p>
                <p><a href='https://www.instagram.com/ginilog?igsh=NTc4MTIwNjQ2YQ==' style='color: #0046BE; text-decoration: none;'>Instagram</a></p>
            </div>

            <br/>
            <h3 style='font-size: 1.2rem; margin: 0; color: #1434A4;'>Get in touch</h3>
            <p style='font-size: 1rem; margin: 0.3em 0;'>+234 816 651 6944</p>
            <p style='font-size: 1rem; margin: 0.3em 0;'><a href='mailto:info@ginilog.com' style='color: #0046BE; text-decoration: none;'>info@ginilog.com</a></p>
            <p style='font-size: 12px; color: #999;'>Copyright © Ginilog Ltd. All Rights Reserved.</p>
           </div>
               </body>
             </html>";

            await SendAsync(emailId, subject, body);
        }

        public static async Task SendChangePasswordCodeEmail(string emailId, string activationcode, string username)
        {
            string subject = "Password Recovery Code";

            string body = $@"
             <html>
             <body style='font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; text-align: center;'>
             <div style='max-width: 600px; margin: auto; background: white; padding: 20px; border-radius: 5px;'>

             <h2 style='color: #0046BE; margin: 0;'>Ginilog Ltd</h2>
             <hr style='border: 1px solid #ddd;'>

             <h2 style='color: #0046BE;'>Password Recovery Code</h2>
             <p style='font-size: 16px;'>Hi {username},</p>
             <p style='color: #666;'>Forgot your password? Recover it now by using the code below:</p>

             <div style='background-color: #ff6600; color: white; padding: 12px 20px; font-size: 16px;
                    border-radius: 5px; display: inline-block; font-weight: bold;'>
            {activationcode}
             </div>

             <br/><br/>
             <p style='font-size: 14px; color: #888;'>This code expires in 10 minutes.</p>

             <br/><br/>
            <p style='font-size: 1.18rem;'>Thanks, <br> Ginilog Team</p>

             <br/><br/>
              <div style='border-top: 1px solid #ddd; padding-top: 10px; text-align: left;'>
             <p style='color: #666; font-weight: bold;'>Follow us on:</p>
             <p><a href='https://www.facebook.com/profile.php?id=61565116785067' style='color: #0046BE; text-decoration: none;'>Facebook</a></p>
             <p><a href='https://twitter.com/' style='color: #0046BE; text-decoration: none;'>Twitter</a></p>
             <p><a href='https://www.instagram.com/ginilog?igsh=NTc4MTIwNjQ2YQ==' style='color: #0046BE; text-decoration: none;'>Instagram</a></p>
             </div>

             <br/>
             <h3 style='font-size: 1.2rem; margin: 0; color: #1434A4;'>Get in touch</h3>
             <p style='font-size: 1rem; margin: 0.3em 0;'>+234 816 651 6944</p>
             <p style='font-size: 1rem; margin: 0.3em 0;'><a href='mailto:info@ginilog.com' style='color: #0046BE; text-decoration: none;'>info@ginilog.com</a></p>
             <p style='font-size: 12px; color: #999;'>Copyright © Ginilog Ltd. All Rights Reserved.</p>
             </div>
             </body>
             </html>";

            await SendAsync(emailId, subject, body);
        }

        public static async Task SendEmail(string emailId, string content, string title, string username, string link)
        {
            string subject = title;

            string body = $@"
            <html>
             <body style='font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; text-align: center;'>
            <div style='max-width: 600px; margin: auto; background: white; padding: 20px; border-radius: 5px;'>

            <h2 style='color: #0046BE; margin: 0;'>Ginilog Ltd</h2>
            <hr style='border: 1px solid #ddd;'>

            <h2 style='color: #0046BE;'>{title}</h2>
            <p style='font-size: 16px;'>Hi {username},</p>
            <p style='font-size: 1rem; line-height: 30px; word-wrap: break-word; word-break: break-word;'>
            {content}
            </p>
            {(string.IsNullOrEmpty(link) ? "" : $"<a href='{link}' style='display: inline-block; padding: 12px 25px; font-size: 1.2rem; background-color: #1434A4; color: #ffffff; text-decoration: none; border-radius: 5px; margin-top: 15px;'>Click Here</a>")}
            <p style='font-size: 1.18rem;'>Thanks, <br> Ginilog Team</p>

            <br/>
             <div style='border-top: 1px solid #ddd; padding-top: 10px; text-align: left;'>
            <p style='color: #666; font-weight: bold;'>Follow us on:</p>
            <p><a href='https://www.facebook.com/profile.php?id=61565116785067' style='color: #0046BE; text-decoration: none;'>Facebook</a></p>
            <p><a href='https://twitter.com/' style='color: #0046BE; text-decoration: none;'>Twitter</a></p>
            <p><a href='https://www.instagram.com/ginilog?igsh=NTc4MTIwNjQ2YQ==' style='color: #0046BE; text-decoration: none;'>Instagram</a></p>
             </div>

              <br/>
             <h3 style='font-size: 1.2rem; margin: 0; color: #1434A4;'>Get in touch</h3>
            <p style='font-size: 1rem; margin: 0.3em 0;'>+234 816 651 6944</p>
            <p style='font-size: 1rem; margin: 0.3em 0;'><a href='mailto:info@ginilog.com' style='color: #0046BE; text-decoration: none;'>info@ginilog.com</a></p>
            <p style='font-size: 12px; color: #999;'>Copyright © Ginilog Ltd. All Rights Reserved.</p>
            </div>
             </body>
            </html>";

            await SendAsync(emailId, subject, body);
        }
    }
}
