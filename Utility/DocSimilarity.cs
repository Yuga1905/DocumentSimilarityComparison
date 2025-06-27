using DocumentSimilarityComparison.Models;
using System;

namespace DocumentSimilarityComparison.Utility
{
    public class DocSimilarity
    {
        private readonly DocumentDbContext _context;

        public DocSimilarity(DocumentDbContext context)
        {
            _context = context;
        }

        public async Task<Resume_Details_Model> CreateResumeAsync(Resume_Details_Model resumeDetail)
        {
            _context.ResumeDetails.Add(resumeDetail);
            await _context.SaveChangesAsync();
            return resumeDetail;
        }

        public async Task<Job_Description_Model> CreateResumeAsync(Job_Description_Model jobDescriptionDetail)
        {
            _context.JobDescriptions.Add(jobDescriptionDetail);
            await _context.SaveChangesAsync();
            return jobDescriptionDetail;
        }

    }
}
