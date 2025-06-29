using DocumentSimilarityComparison.AzureHelper;
using DocumentSimilarityComparison.DTO;
using DocumentSimilarityComparison.Models;

namespace DocumentSimilarityComparison.AgentHelper
{
    public static class ComparisonAgent
    {
        public static async Task<JobDescriptionDTO> MatchResumesWithJobDescription(string jobDescriptionPath)
        {
            JobDescriptionDTO jobDescriptionDTO = new JobDescriptionDTO();
            string folderPath = @"C:\Users\1000055632\source\repos\DocumentSimilarityComparison\DocumentSimilarityComparison\Resources\JobApplicantsResume";
            string[] pdfFiles = Directory.GetFiles(folderPath, "*.pdf");
            Job_Description_Model job_Description_Model=new Job_Description_Model();
            (string resultJobDescriptionText, job_Description_Model) = await AzureHelper.AzureAIClientService.GetJobDescription(jobDescriptionPath, job_Description_Model);
            foreach (string pdfPath in pdfFiles)
            {
                ResumeDTO resumeDTO = new ResumeDTO();
                resumeDTO.PdfPath = pdfPath;
                await AzureHelper.AzureAIClientService.GetComparisonScoreAsync(pdfPath, resumeDTO, resultJobDescriptionText, job_Description_Model);
                jobDescriptionDTO.Resumes.Add(resumeDTO);
            }
            jobDescriptionDTO.JdID = job_Description_Model.JdId;            
            return jobDescriptionDTO;
        }
    }
}
