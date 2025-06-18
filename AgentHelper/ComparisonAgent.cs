using DocumentSimilarityComparison.AzureHelper;
using DocumentSimilarityComparison.DTO;

namespace DocumentSimilarityComparison.AgentHelper
{
    public static class ComparisonAgent
    {
        public static async Task<List<ResumeDTO>> MatchResumesWithJobDescription(List<ResumeDTO> resumes, string jobDescription)
        {
            List<ResumeDTO> shortlistedResumes = new List<ResumeDTO>();
            foreach (ResumeDTO resume in resumes)
            {
                resume.ProfileScore = await AzureHelper.AzureAIClientService.GetComparisonScoreAsync(resume.JobDescription, jobDescription);
                resume.ProfileMatchingPercentage = resume.ProfileScore.ToString("P");
                if (resume.ProfileScore >= 0.8)
                {
                    shortlistedResumes.Add(resume);
                }
            }
            return shortlistedResumes;
        }
    }
}
