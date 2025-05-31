using Microsoft.AspNetCore.Mvc;

namespace StudyBuddyMVC.DTOs
{
    public class UserSessionDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
        

    }
}
