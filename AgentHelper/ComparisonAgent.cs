using DocumentSimilarityComparison.AzureHelper;

namespace DocumentSimilarityComparison.AgentHelper
{
    public static class ComparisonAgent
    {
        public static async Task<List<string>> MatchResumesWithJobDescription(List<string> resumes, string jobDescription)
        {
            List<string> shortlistedResumes = new List<string>();
            List<double> shortlistedScore = new List<double>();
            foreach (var resume in resumes)
            {
                //double comparisonScore = await AzureHelper.AzureAIClientService.GetComparisonScoreAsync(resume,jobDescription);
                double comparisonScore = await AzureHelper.AzureAIClientService.GetComparisonScoreAsync(resume, jobDescription);
                if (comparisonScore>=0.8)
                {
                    shortlistedResumes.Add(resume);
                    shortlistedScore.Add(comparisonScore);
                }
            }
            return shortlistedResumes;
        }
    }
}
