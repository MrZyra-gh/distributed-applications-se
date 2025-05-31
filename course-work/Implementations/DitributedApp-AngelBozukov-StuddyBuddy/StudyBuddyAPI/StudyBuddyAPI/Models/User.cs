using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace StudyBuddyAPI.Models
{   
    public class User : IdentityUser
    {

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Role {  get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }    

    }
}
