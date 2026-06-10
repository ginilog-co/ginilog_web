using System.Net;
using System.Net.Http.Json;
using System.Net.Mail;

namespace Genilog_WebApi.EmailSender
{
    public class EmailTemplates
    {
    
        public static void SendEmailVerificationCode(string emailId, string activationcode, string username)
        {
            var fromMail = new MailAddress("verification@ginilog.org", "Ginilog");
            var toMail = new MailAddress(emailId);
            var frontEmailPassword = "Ginilog21$";
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
                <p style='color: #666; font-weight: bold;'>Download our app:</p>
                <p><a href='https://apps.apple.com/us/app/ginilog/id6751465248' style='color: #0046BE; text-decoration: none;'>Apple Store</a></p>
                <p><a href='https://play.google.com/store/apps/details?id=com.app.ginilog_customer_app' style='color: #0046BE; text-decoration: none;'>Google Play Store</a></p>

                <p style='color: #666; font-weight: bold;'>Follow us on:</p>
                <p><a href='https://web.facebook.com/profile.php?id=61563047290273' style='color: #0046BE; text-decoration: none;'>Facebook</a></p>
                <p><a href='https://x.com/Ginilog_co?s=20' style='color: #0046BE; text-decoration: none;'>Twitter</a></p>
                <p><a href='https://www.instagram.com/ginilog.co?igsh=MWc0ZnFqa2U1cHpqNA==' style='color: #0046BE; text-decoration: none;'>Instagram</a></p>
                <p><a href='https://www.tiktok.com/@ginilog1' style='color: #0046BE; text-decoration: none;'>Tiktok</a></p>
            </div>

            <br/>
            <h3 style='font-size: 1.2rem; margin: 0; color: #1434A4;'>Get in touch</h3>
            <p style='font-size: 1rem; margin: 0.3em 0;'>+234 808 968 4857</p>
            <p style='font-size: 1rem; margin: 0.3em 0;'><a href='mailto:info@ginilog.org' style='color: #0046BE; text-decoration: none;'>info@ginilog.org</a></p>
            <p style='font-size: 12px; color: #999;'>Copyright © Ginilog Ltd. All Rights Reserved.</p>
           </div>
               </body>
             </html>";

            var smtp = new SmtpClient
            {
                Host = "mail.ginilog.org",
                Port = 8889,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromMail.Address, frontEmailPassword)
            };

            using var message = new MailMessage(fromMail, toMail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                // AlternateViews = { AlternateView.CreateAlternateViewFromString(body2, null, "text/plain") }
            };

            smtp.Send(message);
        }

        public static void SendChangePasswordCodeEmail(string emailId, string activationcode, string username)
        {
            var fromMail = new MailAddress("verification@ginilog.org", "Ginilog");
            var toMail = new MailAddress(emailId);
            var frontEmailPassword = "Ginilog21$";
            string subject = "Password Recovery Code";

            string companyHeader = @"
            <div style='display: flex; align-items: center; justify-content: center;'>
            <h2 style='color: #0046BE; margin: 0;'>Ginilog Ltd</h2>
             </div>
             <hr style='border: 1px solid #ddd;'>";

            string appStoreLink = "<a href='https://apps.apple.com/us/app/ginilog/id6751465248' style='color: #0046BE; text-decoration: none;'>Apple Store</a>";
            string playStoreLink = "<a href='https://play.google.com/store/apps/details?id=com.app.ginilog_customer_app' style='color: #0046BE; text-decoration: none;'>Google Play Store</a>";
            string facebookLink = "<a href='https://web.facebook.com/profile.php?id=61563047290273' style='color: #0046BE; text-decoration: none;'>Facebook</a>";
            string twitterLink = "<a href='https://x.com/Ginilog_co?s=20' style='color: #0046BE; text-decoration: none;'>Twitter</a>";
            string instagramLink = "<a href='https://www.instagram.com/ginilog.co?igsh=MWc0ZnFqa2U1cHpqNA==' style='color: #0046BE; text-decoration: none;'>Instagram</a>";
            string tiktokLink = "<a href='https://www.tiktok.com/@ginilog1' style='color: #0046BE; text-decoration: none;'>Tiktok</a>";

            string body = $@"
             <html>
             <body style='font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; text-align: center;'>
             <div style='max-width: 600px; margin: auto; background: white; padding: 20px; border-radius: 5px;'>

              {companyHeader}

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
              <p style='color: #666; font-weight: bold;'>Download our app:</p>
               <p>{appStoreLink}</p>
             <p>{playStoreLink}</p>

             <p style='color: #666; font-weight: bold;'>Follow us on:</p>
             <p>{facebookLink}</p>
             <p>{twitterLink}</p>
             <p>{instagramLink}</p>
             <p>{tiktokLink}</p>
             </div>

             <br/>
             <h3 style='font-size: 1.2rem; margin: 0; color: #1434A4;'>Get in touch</h3>
             <p style='font-size: 1rem; margin: 0.3em 0;'>+234 808 968 4857</p>
             <p style='font-size: 1rem; margin: 0.3em 0;'><a href='mailto:info@ginilog.org' style='color: #0046BE; text-decoration: none;'>info@ginilog.org</a></p>
             <p style='font-size: 12px; color: #999;'>Copyright © Ginilog Ltd. All Rights Reserved.</p>
             </div>
             </body>
             </html>";

            var smtp = new SmtpClient
            {
                Host = "mail.ginilog.org",
                Port = 8889,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromMail.Address, frontEmailPassword)
            };

            using var message = new MailMessage(fromMail, toMail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            smtp.Send(message);
        }

        public static void SendEmail(string emailId, string content, string title, string username, string link)
        {
            var fromMail = new MailAddress("support@ginilog.org", "Ginilog");
            var toMail = new MailAddress(emailId);
            var frontEmailPassword = "Ginilog21$";
            string subject = $"{title}";

            string companyHeader = @"
            <div style='display: flex; align-items: center; justify-content: center;'>
           <h2 style='color: #0046BE; margin: 0;'>Ginilog Ltd</h2>
           </div>
           <hr style='border: 1px solid #ddd;'>";

            string appStoreLink = "<a href='https://apps.apple.com/us/app/ginilog/id6751465248' style='color: #0046BE; text-decoration: none;'>Apple Store</a>";
            string playStoreLink = "<a href='https://play.google.com/store/apps/details?id=com.app.ginilog_customer_app' style='color: #0046BE; text-decoration: none;'>Google Play Store</a>";
            string facebookLink = "<a href='https://web.facebook.com/profile.php?id=61563047290273' style='color: #0046BE; text-decoration: none;'>Facebook</a>";
            string twitterLink = "<a href='https://x.com/Ginilog_co?s=20' style='color: #0046BE; text-decoration: none;'>Twitter</a>";
            string instagramLink = "<a href='https://www.instagram.com/ginilog.co?igsh=MWc0ZnFqa2U1cHpqNA==' style='color: #0046BE; text-decoration: none;'>Instagram</a>";
            string tiktokLink = "<a href='https://www.tiktok.com/@ginilog1' style='color: #0046BE; text-decoration: none;'>Tiktok</a>";

            string body = $@"
            <html>
             <body style='font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; text-align: center;'>
            <div style='max-width: 600px; margin: auto; background: white; padding: 20px; border-radius: 5px;'>

            {companyHeader}

            <h2 style='color: #0046BE;'>{title}</h2>
            <p style='font-size: 16px;'>Hi {username},</p>
            <p style='font-size: 1rem; line-height: 30px; word-wrap: break-word; word-break: break-word;'>
            {content}
            </p>
            <a href='{link}' style='display: inline-block; padding: 12px 25px; font-size: 1.2rem; background-color: #1434A4; color: #ffffff; text-decoration: none; border-radius: 5px; margin-top: 15px;'>
            Click Here
            </a>
            <p style='font-size: 1.18rem;'>Thanks, <br> Ginilog Team</p>

            <br/><br/>

             <div style='border-top: 1px solid #ddd; padding-top: 10px; text-align: left;'>
            <p style='color: #666; font-weight: bold;'>Download our app:</p>
            <p>{appStoreLink}</p>
            <p>{playStoreLink}</p>

            <p style='color: #666; font-weight: bold;'>Follow us on:</p>
            <p>{facebookLink}</p>
            <p>{twitterLink}</p>
            <p>{instagramLink}</p>
            <p>{tiktokLink}</p>
             </div>

              <br/>
             <h3 style='font-size: 1.2rem; margin: 0; color: #1434A4;'>Get in touch</h3>
            <p style='font-size: 1rem; margin: 0.3em 0;'>+234 808 968 4857</p>
            <p style='font-size: 1rem; margin: 0.3em 0;'><a href='mailto:info@ginilog.org' style='color: #0046BE; text-decoration: none;'>info@ginilog.org</a></p>
            <p style='font-size: 12px; color: #999;'>Copyright © Ginilog Ltd. All Rights Reserved.</p>
            </div>
             </body>
            </html>";


            var smtp = new SmtpClient
            {
                Host = "mail.ginilog.org",
                Port = 8889,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromMail.Address, frontEmailPassword)
            };

            using var message = new MailMessage(fromMail, toMail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            smtp.Send(message);
        }

      
    }
}
