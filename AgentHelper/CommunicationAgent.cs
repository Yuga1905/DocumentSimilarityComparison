using Azure.Identity;
using DocumentSimilarityComparison.DTO;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using System.Net;
using System.Net.Mail;

namespace DocumentSimilarityComparison.AgentHelper
{
    public static class CommunicationAgent
    {
        public static async Task<string> SendEmailWithRank(JobDescriptionDTO jobDescription,string requestorMailId)
        {
            try
            {
                ResumeDTO resumeDTO = jobDescription.Resumes.FirstOrDefault();

                string fromEmail = "yugashini1905@gmail.com";
                string fromEmailPassword = "ztvh ixgs guyf iauo";
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(fromEmail);
                mailMessage.To.Add(requestorMailId);
                mailMessage.Subject = string.Format("Top Applicants for {0}",jobDescription.JobTitle);
                mailMessage.Body = "Dear AR Requestor\n";
                mailMessage.Body += "Please find the shortlisted resume in the attachment\n\n";

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.Credentials = new NetworkCredential(fromEmail, fromEmailPassword);
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                foreach (ResumeDTO resume in jobDescription.Resumes)
                {
                    if (resume.Rank >= 1 && resume.Rank <= 3)
                    {
                        mailMessage.Body += string.Format("{0} Profile matched with {1}%. {2}\n\n",resume.ApplicantName, Math.Round((decimal)(resume.ProfileScore * 100), 2), resume.Summary);
                    }
                }
                foreach (ResumeDTO resume in jobDescription.Resumes)
                {
                    if(resume.Rank>=1 && resume.Rank<=3)
                    {
                        System.Net.Mail.Attachment pdfAttachment = new System.Net.Mail.Attachment(resume.PdfPath);
                        mailMessage.Attachments.Add(pdfAttachment);
                    }
                }
                

                smtpClient.Send(mailMessage);

                return "Communication Sent";

            }
            catch (Exception ex)
            {
                return "Communication Failed";
            }
        }
    }
}
