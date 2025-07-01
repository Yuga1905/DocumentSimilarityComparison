using DocumentSimilarityComparison.AzureHelper;
using DocumentSimilarityComparison.DTO;
using DocumentSimilarityComparison.Models;
using System.IO;
using System.Threading.Tasks;

namespace DocumentSimilarityComparison.AgentHelper
{
    public static class ComparisonAgent
    {
        public static async Task<JobDescriptionDTO> MatchResumesWithJobDescription(string jobDescriptionPath, string resumesFolderPath)
        {
            JobDescriptionDTO jobDescriptionDTO = new JobDescriptionDTO();

            // Ensure the folder exists
            if (!Directory.Exists(resumesFolderPath))
                return jobDescriptionDTO; // or throw error

            string[] pdfFiles = Directory.GetFiles(resumesFolderPath, "*.pdf");

            Job_Description_Model job_Description_Model = new Job_Description_Model();
            (string resultJobDescriptionText, job_Description_Model) =
                await AzureHelper.AzureAIClientService.GetJobDescription(jobDescriptionPath, job_Description_Model);

            foreach (string pdfPath in pdfFiles)
            {
                ResumeDTO resumeDTO = new ResumeDTO
                {
                    PdfPath = pdfPath
                };
                await AzureHelper.AzureAIClientService.GetComparisonScoreAsync(pdfPath, resumeDTO, resultJobDescriptionText, job_Description_Model);
                jobDescriptionDTO.Resumes.Add(resumeDTO);
            }

            jobDescriptionDTO.JdID = job_Description_Model.JdId;
            jobDescriptionDTO.JobTitle = job_Description_Model.JdTitle;

            return jobDescriptionDTO;
        }
    }
}