using DocumentSimilarityComparison.DTO;
using System.Collections.Generic;

namespace DocumentSimilarityComparison.AgentHelper
{
    public static class RankingAgent
    {
        public static async Task<JobDescriptionDTO> RankResumesWithScore(JobDescriptionDTO resumes)
        {
            int rankCount = 1;
            JobDescriptionDTO rankedResumes = new JobDescriptionDTO();
            foreach (ResumeDTO resume in resumes.Resumes.OrderByDescending(x => x.ProfileScore).ToList())
            {
                resume.Rank = rankCount;
                rankCount++;
                rankedResumes.Resumes.Add(resume);
            }
            rankedResumes.JdID = resumes.JdID;
            return rankedResumes;
        }
    }
}
