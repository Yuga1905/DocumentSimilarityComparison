using DocumentSimilarityComparison.DTO;
using DocumentSimilarityComparison.Models;

namespace DocumentSimilarityComparison.Utility
{
    public class ResumeAnalyzeHelper
    {
        internal void PopulateResumeDTO(Resume_Details_Model resume_Details, ResumeDTO resumedto,Job_Description_Model job_Description_Model)
        {
            resume_Details.Name = resumedto.ApplicantName;
            resume_Details.Email = resumedto.ApplicantEmailId;
            resume_Details.Skills = resumedto.SkillsText;
            resume_Details.JdId = job_Description_Model.JdId;
            resume_Details.Experience = resumedto.Experience;
            resume_Details.Score = Math.Round((decimal)(resumedto.ProfileScore * 100), 2);
        }
    }
}
