namespace DocumentSimilarityComparison.Models
{
    public class Requestor_Model
    {
        public int Id { get; set; }            // PK (Add this for entity identity)

        public int JdId { get; set; }           // FK
        public string ComparisonStatus { get; set; }
        public string CommunicationStatus { get; set; }

        // Navigation Property
        public Job_Description_Model JobDescription { get; set; }
    }
}
