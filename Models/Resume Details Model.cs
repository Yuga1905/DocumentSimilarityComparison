namespace DocumentSimilarityComparison.Models
{
    public class Resume_Details_Model
    {
        public int Id { get; set; }             // PK (Add this for entity identity)

        public int JdId { get; set; }            // FK
        public string Name { get; set; }
        public string Email { get; set; }
        public int Experience { get; set; }       // Could be in years or months
        public decimal Score { get; set; }        // or double
        public string Skills { get; set; }        // Comma-separated or move to another table for normalization

        public string UserCommunicationStatus { get; set; }

        // Navigation Property
        public Job_Description_Model JobDescription { get; set; }
    }
}
