using System.Net.Mail;
using System.Net;

namespace Genilog_WebApi.EmailSender
{
    public class EmailTemplates
    {
        public static void SendEmailVerificationCode(string emailId, string activationcode, string username)
        {

            var fromMail = new MailAddress("verification@bringmygas.com", "BMG(Bring My Gas) App");
            var toMail = new MailAddress(emailId);
            var frontEmailPassowrd = "Favour1990$";
            string subject = " Email Verification Token";
            string body = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Bring My Gas OTP Verification</title>
    <link rel=""preconnect"" href=""https://fonts.googleapis.com"">
    <link rel=""preconnect"" href=""https://fonts.gstatic.com"" crossorigin>
    <link href=""https://fonts.googleapis.com/css2?family=Poppins:wght@100;200;300;400;500;600;700;800;900&display=swap"" rel=""stylesheet"">
</head>
<body style=""margin: 0; padding: 0; font-family: 'Poppins', sans-serif; background-color: #f4f4f4;"">
    <div style=""max-width: 700px; margin: 3em auto; background-color: #ffffff;"">
        <header style=""text-align: center; padding: 1.2em 0; background-color: #fff;"">
            <img src=""https://firebasestorage.googleapis.com/v0/b/bmg-project-edf2f.appspot.com/o/Assets%2Fplaystore-icon.png?alt=media&token=8f700536-b14d-4f3b-b35b-0275963461cd"" alt=""Your Site Logo"" style=""width: 30%; height: 30%;"">
        </header>

        <section style=""background-color:#1434A4; text-align: center; padding: 1em; letter-spacing: 5px;"">
            <img src=""https://firebasestorage.googleapis.com/v0/b/bmg-project-edf2f.appspot.com/o/Assets%2Fmessage.png?alt=media&token=8188a060-ebc4-4f53-9bef-829130610ddd"" alt=""Email Icon"" style=""margin-top: 1.3em; width: 30%; height: 30%;"">
            <h2 style=""color: #f4f4f4; font-size: 1.1rem; font-weight: 700;"">THANKS FOR SIGNING UP!</h2>
            <h3 style=""color: #f4f4f4; font-size: 2rem; font-weight: 700;"">OTP</h3>
            <p style=""color: #f4f4f4; font-size: 1.5rem; font-weight: 400;"">{activationcode}</p>
        </section>

        <section style=""padding: 3em; text-align: center; background-color: #fff; font-weight: 500;"">
            <p style=""font-size: 1.26rem;"">Hi {username},</p>
            <p style=""font-size: 1rem; line-height: 30px; word-wrap: break-word; word-break: break-word;"">
                You're almost ready to get started. Please copy the OTP above to verify your email address and start ordering your gas from us!
            </p>
            <p style=""font-size: 1.18rem;"">Thanks, <br> BMG Team</p>
        </section>

        <footer style=""background-color: #CCCCFF; padding-top: 3em; padding-bottom:2em; text-align: center;"">
            <h3 style=""font-size: 1.2rem; margin: 0; color: #1434A4;"">Get in touch</h3>
            <p style=""font-size: 1rem; margin: 0.3em 0;"">+234 816 651 6944</p>
            <p style=""font-size: 1rem; margin: 0.3em 0;"">info@bringmygas.com</p>
            <div style=""margin-top: 2em;"">
                <a href=""#"" style=""text-decoration: none; margin-right: 0.9em;"">
                    <img src=""https://firebasestorage.googleapis.com/v0/b/bmg-project-edf2f.appspot.com/o/Assets%2Ffacebook.png?alt=media&token=ad4a1a52-bce7-44f5-8d3e-6b4dcfcf3bba"" alt=""Facebook"" style=""width: 35px;"">
                </a>
                <a href=""#"" style=""text-decoration: none; margin-right: 0.9em;"">
                    <img src=""https://firebasestorage.googleapis.com/v0/b/bmg-project-edf2f.appspot.com/o/Assets%2Flinkedin.png?alt=media&token=91e66472-b87c-4572-bdc7-46a4a3920800"" alt=""LinkedIn"" style=""width: 35px;"">
                </a>
                <a href=""#"" style=""text-decoration: none; margin-right: 0.9em;"">
                    <img src=""https://firebasestorage.googleapis.com/v0/b/bmg-project-edf2f.appspot.com/o/Assets%2Finstagram.png?alt=media&token=86db909b-d260-45c4-b957-bfa05c616d44"" alt=""Instagram"" style=""width: 35px;"">
                </a>
            </div>
        </footer>

        <section style=""background-color: #1434A4; padding: 1.2em; text-align: center;"">
            <p style=""color: #ffffff; font-size: 1.1rem; margin: 0;"">Copyrights © Bring My Gas All Rights Reserved</p>
        </section>
    </div>
</body>
</html>";
            var smtp = new SmtpClient
            {
                Host = "mail.cntechlite.com",
                Port = 8889,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromMail.Address, frontEmailPassowrd)
            };
            using var message = new MailMessage(fromMail, toMail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            smtp.Send(message);
        }

        public static void SendChangePasswordCodeEmail(string emailId, string activationcode, string username)
        {

            var fromMail = new MailAddress("verification@bringmygas.com", "BMG(Bring My Gas) App");
            var toMail = new MailAddress(emailId);
            var frontEmailPassowrd = "Favour1990$";
            string subject = " Email Verification Token";
            string body = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Bring My Gas OTP Verification</title>
    <link rel=""preconnect"" href=""https://fonts.googleapis.com"">
    <link rel=""preconnect"" href=""https://fonts.gstatic.com"" crossorigin>
    <link href=""https://fonts.googleapis.com/css2?family=Poppins:wght@100;200;300;400;500;600;700;800;900&display=swap"" rel=""stylesheet"">
</head>
<body style=""margin: 0; padding: 0; font-family: 'Poppins', sans-serif; background-color: #f4f4f4;"">
    <div style=""max-width: 700px; margin: 3em auto; background-color: #ffffff;"">
        <header style=""text-align: center; padding: 1.2em 0; background-color: #fff;"">
            <img src=""https://firebasestorage.googleapis.com/v0/b/bmg-project-edf2f.appspot.com/o/Assets%2Fplaystore-icon.png?alt=media&token=8f700536-b14d-4f3b-b35b-0275963461cd"" alt=""Your Site Logo"" style=""width: 30%; height: 30%;"">
        </header>

        <section style=""background-color:#1434A4; text-align: center; padding: 1em; letter-spacing: 5px;"">
            <img src=""https://firebasestorage.googleapis.com/v0/b/bmg-project-edf2f.appspot.com/o/Assets%2Fmessage.png?alt=media&token=8188a060-ebc4-4f53-9bef-829130610ddd"" alt=""Email Icon"" style=""margin-top: 1.3em; width: 30%; height: 30%;"">
            <h2 style=""color: #f4f4f4; font-size: 1.1rem; font-weight: 700;"">Reset Password!</h2>
            <h3 style=""color: #f4f4f4; font-size: 2rem; font-weight: 700;"">OTP</h3>
            <p style=""color: #f4f4f4; font-size: 1.5rem; font-weight: 400;"">{activationcode}</p>
        </section>

        <section style=""padding: 3em; text-align: center; background-color: #fff; font-weight: 500;"">
            <p style=""font-size: 1.26rem;"">Hi {username},</p>
            <p style=""font-size: 1rem; line-height: 30px; word-wrap: break-word; word-break: break-word;"">
                You're almost ready to get started. Please copy the OTP above and reset your password!
            </p>
            <p style=""font-size: 1.18rem;"">Thanks, <br> BMG Team</p>
        </section>

        <footer style=""background-color: #CCCCFF; padding-top: 3em; padding-bottom:2em; text-align: center;"">
            <h3 style=""font-size: 1.2rem; margin: 0; color: #1434A4;"">Get in touch</h3>
            <p style=""font-size: 1rem; margin: 0.3em 0;"">+234 816 651 6944</p>
            <p style=""font-size: 1rem; margin: 0.3em 0;"">info@bringmygas.com</p>
            <div style=""margin-top: 2em;"">
                <a href=""#"" style=""text-decoration: none; margin-right: 0.9em;"">
                    <img src=""https://firebasestorage.googleapis.com/v0/b/bmg-project-edf2f.appspot.com/o/Assets%2Ffacebook.png?alt=media&token=ad4a1a52-bce7-44f5-8d3e-6b4dcfcf3bba"" alt=""Facebook"" style=""width: 35px;"">
                </a>
                <a href=""#"" style=""text-decoration: none; margin-right: 0.9em;"">
                    <img src=""https://firebasestorage.googleapis.com/v0/b/bmg-project-edf2f.appspot.com/o/Assets%2Flinkedin.png?alt=media&token=91e66472-b87c-4572-bdc7-46a4a3920800"" alt=""LinkedIn"" style=""width: 35px;"">
                </a>
                <a href=""#"" style=""text-decoration: none; margin-right: 0.9em;"">
                    <img src=""https://firebasestorage.googleapis.com/v0/b/bmg-project-edf2f.appspot.com/o/Assets%2Finstagram.png?alt=media&token=86db909b-d260-45c4-b957-bfa05c616d44"" alt=""Instagram"" style=""width: 35px;"">
                </a>
            </div>
        </footer>

        <section style=""background-color: #1434A4; padding: 1.2em; text-align: center;"">
            <p style=""color: #ffffff; font-size: 1.1rem; margin: 0;"">Copyrights © Bring My Gas All Rights Reserved</p>
        </section>
    </div>
</body>
</html>";
            var smtp = new SmtpClient
            {
                Host = "mail.cntechlite.com",
                Port = 8889,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromMail.Address, frontEmailPassowrd)
            };
            using var message = new MailMessage(fromMail, toMail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            smtp.Send(message);
        }
      
        public static void SendMail(string emailId, string username, string content)
        {

            var fromMail = new MailAddress("verification@bringmygas.com", "BMG(Bring My Gas) App");
            var toMail = new MailAddress(emailId);
            var frontEmailPassowrd = "Favour1990$";
            string subject = " Email Verification Token";
            string body = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Bring My Gas OTP Verification</title>
    <link rel=""preconnect"" href=""https://fonts.googleapis.com"">
    <link rel=""preconnect"" href=""https://fonts.gstatic.com"" crossorigin>
    <link href=""https://fonts.googleapis.com/css2?family=Poppins:wght@100;200;300;400;500;600;700;800;900&display=swap"" rel=""stylesheet"">
</head>
<body style=""margin: 0; padding: 0; font-family: 'Poppins', sans-serif; background-color: #f4f4f4;"">
    <div style=""max-width: 700px; margin: 3em auto; background-color: #ffffff;"">
        <header style=""text-align: center; padding: 1.2em 0; background-color: #fff;"">
            <img src=""https://firebasestorage.googleapis.com/v0/b/bmg-project-edf2f.appspot.com/o/Assets%2Fplaystore-icon.png?alt=media&token=8f700536-b14d-4f3b-b35b-0275963461cd"" alt=""Your Site Logo"" style=""width: 30%; height: 30%;"">
        </header>

        <section style=""background-color:#1434A4; text-align: center; padding: 1em; letter-spacing: 5px;"">
            <img src=""https://firebasestorage.googleapis.com/v0/b/bmg-project-edf2f.appspot.com/o/Assets%2Fmessage.png?alt=media&token=8188a060-ebc4-4f53-9bef-829130610ddd"" alt=""Email Icon"" style=""margin-top: 1.3em; width: 30%; height: 30%;"">
            <h2 style=""color: #f4f4f4; font-size: 1.1rem; font-weight: 700;"">Here is the Content!</h2>
            <p>Bring My Gas App</p><br/><strong>Hi {username} </strong><br><br/>{content}</p>
        </section>

        <section style=""padding: 3em; text-align: center; background-color: #fff; font-weight: 500;"">
            <p style=""font-size: 1.18rem;"">Thanks, <br> BMG Team</p>
        </section>

        <footer style=""background-color: #CCCCFF; padding-top: 3em; padding-bottom:2em; text-align: center;"">
            <h3 style=""font-size: 1.2rem; margin: 0; color: #1434A4;"">Get in touch</h3>
            <p style=""font-size: 1rem; margin: 0.3em 0;"">+234 816 651 6944</p>
            <p style=""font-size: 1rem; margin: 0.3em 0;"">info@bringmygas.com</p>
            <div style=""margin-top: 2em;"">
                <a href=""#"" style=""text-decoration: none; margin-right: 0.9em;"">
                    <img src=""https://firebasestorage.googleapis.com/v0/b/bmg-project-edf2f.appspot.com/o/Assets%2Ffacebook.png?alt=media&token=ad4a1a52-bce7-44f5-8d3e-6b4dcfcf3bba"" alt=""Facebook"" style=""width: 35px;"">
                </a>
                <a href=""#"" style=""text-decoration: none; margin-right: 0.9em;"">
                    <img src=""https://firebasestorage.googleapis.com/v0/b/bmg-project-edf2f.appspot.com/o/Assets%2Flinkedin.png?alt=media&token=91e66472-b87c-4572-bdc7-46a4a3920800"" alt=""LinkedIn"" style=""width: 35px;"">
                </a>
                <a href=""#"" style=""text-decoration: none; margin-right: 0.9em;"">
                    <img src=""https://firebasestorage.googleapis.com/v0/b/bmg-project-edf2f.appspot.com/o/Assets%2Finstagram.png?alt=media&token=86db909b-d260-45c4-b957-bfa05c616d44"" alt=""Instagram"" style=""width: 35px;"">
                </a>
            </div>
        </footer>

        <section style=""background-color: #1434A4; padding: 1.2em; text-align: center;"">
            <p style=""color: #ffffff; font-size: 1.1rem; margin: 0;"">Copyrights © Bring My Gas All Rights Reserved</p>
        </section>
    </div>
</body>
</html>";
            var smtp = new SmtpClient
            {
                Host = "mail.cntechlite.com",
                Port = 8889,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromMail.Address, frontEmailPassowrd)
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
