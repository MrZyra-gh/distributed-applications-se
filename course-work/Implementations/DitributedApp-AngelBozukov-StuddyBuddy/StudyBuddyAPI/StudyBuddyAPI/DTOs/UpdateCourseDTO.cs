using System.ComponentModel.DataAnnotations;

namespace StudyBuddyAPI.DTOs
{
    public class UpdateCourseDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public int Capacity { get; set; }
    }
}