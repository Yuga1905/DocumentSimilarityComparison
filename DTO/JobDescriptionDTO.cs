using DocumentSimilarityComparison.Models;

namespace DocumentSimilarityComparison.DTO
{
    public class JobDescriptionDTO
    {
        public int JdID { get; set; }
        public string JobTitle { get; set; }
        public List<ResumeDTO> Resumes { get; set; } = new List<ResumeDTO>();
        public Requestor_Model Requestor { get; set; }
    }
}
