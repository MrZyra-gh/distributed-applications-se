using StudyBuddyMVC.DTOs;
using System.Collections.Generic;

namespace StudyBuddyMVC.ViewModels
{
    public class CourseDetailsViewModel
    {
        public CourseDto Course { get; set; }
        public List<AssignmentDto> Assignments { get; set; }
    }
}