using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlateFormaApprentissage.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int NbMaxStudents { get; set; }
        public int ProfessorId { get; set; }
        public virtual User Professor { get; set; }
        public virtual ICollection<User> Students { get; set; } = new HashSet<User>();
        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();
        public virtual ICollection<Registration> Registrations { get; set; } = new HashSet<Registration>();
        

        public Course(string title, string description, int nbMax)
        {
            Title = title;
            Description = description;
            NbMaxStudents = nbMax;
        }
        public Course(string title, string description)
        {
            Title = title;
            Description = description;
        }


    }
}
