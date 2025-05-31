namespace StudyBuddyMVC.DTOs
{

    public class AssignmentDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Instructions { get; set; }

        public DateTime DueDate { get; set; }

        public double MaxScore { get; set; }

        public int CourseId { get; set; }

        public string CourseTitle { get; set; }
    }
}