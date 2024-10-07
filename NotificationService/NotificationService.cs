using System.Net.Mail;
using System.Net;
using Common.Models;

namespace NotificationService
{
    public class NotificationService
    {
        private void SendQuantityUpdateErrorMessage(ProductModel product, int stockQuantityChange)
        {
            string errorMessage = $"Failed to update the quantity of product '{product.Name}'. " +
                                  $"The last known stock quantity is {product.StockQuantity}. " +
                                  $"Admin intervention is required.";

            try
            {
                // Configure your SMTP settings
                string smtpServer = "your-smtp-server.com";
                int smtpPort = 587; // Use the appropriate port for your SMTP server
                string smtpUsername = "your-smtp-username";
                string smtpPassword = "your-smtp-password";

                // Create a new SMTP client
                using (SmtpClient smtpClient = new SmtpClient(smtpServer))
                {
                    smtpClient.Port = smtpPort;
                    smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    smtpClient.EnableSsl = true; // Use SSL if required

                    // Create an email message
                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress("your-email@example.com"),
                        Subject = "Quantity Update Error",
                        Body = errorMessage,
                        IsBodyHtml = false
                    };

                    // Add the recipient's email address
                    mailMessage.To.Add("admin@example.com"); // Replace with the admin's email address

                    // Send the email
                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the email sending process
                Console.WriteLine("Failed to send email: " + ex.Message);
            }
        }
    }
}