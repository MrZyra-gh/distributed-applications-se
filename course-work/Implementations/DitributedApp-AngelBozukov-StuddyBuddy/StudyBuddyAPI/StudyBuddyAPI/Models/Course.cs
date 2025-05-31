using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyBuddyAPI.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

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

        [ForeignKey("InstructorId")]
        public User Instructor { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
