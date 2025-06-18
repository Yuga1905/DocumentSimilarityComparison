using DocumentSimilarityComparison.DTO;
using System.Net;
using System.Net.Mail;

namespace DocumentSimilarityComparison.AgentHelper
{
    public static class CommunicationAgent
    {
        public static async Task<bool> SendEmailWithRank(List<ResumeDTO> resumes)
        {
            ResumeDTO resumeDTO = resumes.FirstOrDefault();
            string fromEmail = "yugashini1905@gmail.com";
            string fromEmailPassword = "test";
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(fromEmail);
                mailMessage.To.Add(resumeDTO.ApplicantEmailId);
                mailMessage.Subject = "Job Opportunity at Our Company";
                mailMessage.Body = "<html><body><h2>Hello!</h2><p>We have a job that might interest you.</p></body></html>";

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com",587);
                smtpClient.Credentials = new NetworkCredential(fromEmail, fromEmailPassword);
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;

                smtpClient.Send(mailMessage);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
