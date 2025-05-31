using System.ComponentModel.DataAnnotations;

namespace StudyBuddyMVC.DTOs
{
    public class CreateCourseDto
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

        [Required]
        public string InstructorId { get; set; }
    }
}