using System.ComponentModel.DataAnnotations;

namespace StudyBuddyAPI.DTOs
{
    public class UpdateAssignmentDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Instructions { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public double MaxScore { get; set; }
    }
}