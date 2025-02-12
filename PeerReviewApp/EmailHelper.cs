using System;
using System.Net.Mail;

namespace PeerReviewApp
{
    public class EmailHelper
    {
        public bool SendEmail(string userEmail, string conformationLink)
        {
            MailMessage message = new MailMessage();
            //need to make sure there is an actual admin email to send things from
            message.From = new MailAddress("admin@peerreview.com");
            message.To.Add(new MailAddress(userEmail));
            message.Subject = "Confirm your Email to finish registration";
            message.IsBodyHtml = true;
            message.Body = conformationLink;

            SmtpClient client = new SmtpClient();
            //replace "yourpassword" with proper credentials after admin email is created
            client.Credentials = new System.Net.NetworkCredential("admin@peerreview.com", "yourpassword");

            //Need to add host domain 
            client.Host = "";
            client.Port = 80;

            try
            {
                client.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                //log the exception
            }
            return false;

        }
    }
}
