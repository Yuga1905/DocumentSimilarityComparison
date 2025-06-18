namespace DocumentSimilarityComparison.DTO
{
    public class ResumeDTO
    {
        public string ApplicantName { get; set; }
        public string JobDescription { get; set; }
        public double ProfileScore { get; set; }
        public string ProfileMatchingPercentage { get; set; }
        public int Rank { get; set; }
        public string ApplicantEmailId { get; set; }
    }
}
