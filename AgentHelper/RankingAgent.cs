using DocumentSimilarityComparison.DTO;
using System.Collections.Generic;

namespace DocumentSimilarityComparison.AgentHelper
{
    public static class RankingAgent
    {
        public static async Task<List<ResumeDTO>> RankResumesWithScore(List<ResumeDTO> resumes)
        {
            int rankCount = 1;
            List<ResumeDTO> rankedResumes = new List<ResumeDTO>();
            resumes = resumes.OrderByDescending(x => x.ProfileScore).ToList();
            foreach(ResumeDTO resume in resumes)
            {
                resume.Rank = rankCount;
                rankCount++;
                rankedResumes.Add(resume);
            }
            return resumes;
        }
    }
}
