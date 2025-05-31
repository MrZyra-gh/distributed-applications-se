using System.ComponentModel.DataAnnotations;


namespace StudyBuddyAPI.DTOs
{
    public class UpdateUserDto
    {
        [Required]
        [MaxLength(30)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }
    }

}
