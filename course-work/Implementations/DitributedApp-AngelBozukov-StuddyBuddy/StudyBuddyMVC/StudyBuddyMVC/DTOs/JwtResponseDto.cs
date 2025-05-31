namespace StudyBuddyMVC.DTOs
{
    public class JwtResponseDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; } 
    }
}
