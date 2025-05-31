namespace StudyBuddyAPI.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int Capacity { get; set; }

        public string InstructorId { get; set; }

        public string InstructorName { get; set; }
    }
}