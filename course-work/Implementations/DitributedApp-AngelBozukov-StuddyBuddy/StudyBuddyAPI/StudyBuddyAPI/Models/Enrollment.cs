using System.ComponentModel.DataAnnotations;

namespace StudyBuddyAPI.Models
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }

        public string? UserId { get; set; }
        public User? User { get; set; }

        public int? CourseId { get; set; }
        public Course? Course { get; set; }

        public DateTime EnrolledOn { get; set; } = DateTime.UtcNow;
    }
}