using Azure.Core;
using System.ComponentModel.DataAnnotations;

namespace DocumentSimilarityComparison.Models
{
    public class Job_Description_Model
    {
        [Key]
        public int JdId { get; set; }           // PK
        public string JdTitle { get; set; }
        public string Description { get; set; }

        // Navigation Properties
        public ICollection<Requestor_Model> Requestors { get; set; }
        public ICollection<Resume_Details_Model> ResumeDetails { get; set; }
    }
}
