using DocumentSimilarityComparison.AzureHelper;
using DocumentSimilarityComparison.DTO;
using DocumentSimilarityComparison.Models;

namespace DocumentSimilarityComparison.AgentHelper
{
    public static class ComparisonAgent
    {
        public static async Task<List<ResumeDTO>> MatchResumesWithJobDescription(string jobDescriptionPath)
        {
            List<ResumeDTO> shortlistedResumes = new List<ResumeDTO>();
            string folderPath = @"C:\Users\1000055632\source\repos\DocumentSimilarityComparison\DocumentSimilarityComparison\Resources\JobApplicantsResume";
            string[] pdfFiles = Directory.GetFiles(folderPath, "*.pdf");
            Job_Description_Model job_Description_Model=new Job_Description_Model();
            (string resultJobDescriptionText, job_Description_Model) = await AzureHelper.AzureAIClientService.GetJobDescription(jobDescriptionPath, job_Description_Model);
            foreach (string pdfPath in pdfFiles)
            {
                ResumeDTO resumeDTO = new ResumeDTO();
                await AzureHelper.AzureAIClientService.GetComparisonScoreAsync(pdfPath, resumeDTO, resultJobDescriptionText, job_Description_Model);
                resumeDTO.ProfileMatchingPercentage = (resumeDTO.ProfileScore * 100).ToString("F2") + "%"; ;
                shortlistedResumes.Add(resumeDTO);
            }
            
            
            return shortlistedResumes;
        }
    }
}
