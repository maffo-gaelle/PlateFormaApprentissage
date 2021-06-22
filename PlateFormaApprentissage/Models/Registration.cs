using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlateFormaApprentissage.Models
{
    public enum RegistrationType
    {
        REGISTERED,
        DISABLED,
        PENDING
    };
    public class Registration
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public RegistrationType Statut { get; set; }
        public virtual User User { get; set; }
        public virtual Course Course { get; set; }


        public Registration(int studentId, int courseId, RegistrationType statut)
        {
            StudentId = studentId;
            CourseId = courseId;
            Statut = statut;
        }
    }
}
