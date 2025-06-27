using DocumentSimilarityComparison.DTO;
using System.Collections.Generic;

namespace DocumentSimilarityComparison.AgentHelper
{
    public static class RankingAgent
    {
        public static async Task<JobDescriptionDTO> RankResumesWithScore(JobDescriptionDTO resumes)
        {
            //int rankCount = 1;
            JobDescriptionDTO rankedResumes = new JobDescriptionDTO();
            rankedResumes.Resumes = rankedResumes.Resumes.OrderByDescending(x => x.ProfileScore).ToList();
            //foreach(ResumeDTO resume in rankedResumes.Resumes)
            //{
            //    resume.Rank = rankCount;
            //    rankCount++;
            //    rankedResumes.Add(resume);
            //}
            rankedResumes.JdID = resumes.JdID;
            return rankedResumes;
        }
    }
}
