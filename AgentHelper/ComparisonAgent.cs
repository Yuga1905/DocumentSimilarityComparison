using DocumentSimilarityComparison.AzureHelper;
using DocumentSimilarityComparison.DTO;

namespace DocumentSimilarityComparison.AgentHelper
{
    public static class ComparisonAgent
    {
        public static async Task<List<ResumeDTO>> MatchResumesWithJobDescription(string jobDescription)
        {
            List<ResumeDTO> shortlistedResumes = new List<ResumeDTO>();
            ResumeDTO resumeDTO = new ResumeDTO();
            string resumePath = "C:\\Users\\1000055632\\source\\repos\\DocumentSimilarityComparison\\DocumentSimilarityComparison\\Resources\\JobApplicantsResume\\Resume1.pdf";
            await AzureHelper.AzureAIClientService.GetComparisonScoreAsync(resumePath, resumeDTO, jobDescription);
            resumeDTO.ProfileMatchingPercentage = (resumeDTO.ProfileScore * 100).ToString("F2") + "%"; ;
            shortlistedResumes.Add(resumeDTO);
            //foreach (ResumeDTO resume in resumes)
            //{
            //    resume.ProfileScore = await AzureHelper.AzureAIClientService.GetComparisonScoreAsync(resume, jobDescription);
            //    resume.ProfileMatchingPercentage = (resume.ProfileScore*100).ToString();
            //    shortlistedResumes.Add(resume);
            //}
            return shortlistedResumes;
        }
    }
}
