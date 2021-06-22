using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlateFormaApprentissage.Models
{
    public enum Role
    {
        Student,
        Professor
    }

    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public Role Role { get; set; }
        public virtual ICollection<Registration> Registrations { get; set; } = new HashSet<Registration>();
        public virtual ICollection<Course> Courses { get; set; } = new HashSet<Course>();
        public User(string firstName, string lastName, string email, string password, Role role)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            Role = role;
        }
    }
    
}
