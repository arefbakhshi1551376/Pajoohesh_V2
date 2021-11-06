using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Pajoohesh_V2.Utility
{
    public class SendMessages
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using (var Client = new SmtpClient())
            {
                var Credential = new NetworkCredential
                {
                    UserName = "pajoohesh.abbasgholikhan",
                    Password = "pajoohesh_abbasgholikhan_1400"
                };
                Client.Credentials = Credential;
                Client.Host = "smtp.gmail.com";
                Client.Port = 587; // or 25  -- 587 -- 465 For Send Email
                Client.EnableSsl = true;
                using (var emailMessage = new MailMessage())
                {
                    emailMessage.To.Add(new MailAddress(email));
                    emailMessage.From = new MailAddress("pajoohesh.abbasgholikhan@gmail.com");
                    emailMessage.Subject = subject;
                    emailMessage.IsBodyHtml = true;
                    emailMessage.Body = message;

                    Client.Send(emailMessage);
                };
                await Task.CompletedTask;
            }
        }
    }
}
