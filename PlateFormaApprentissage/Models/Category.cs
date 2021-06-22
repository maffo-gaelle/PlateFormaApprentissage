using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlateFormaApprentissage.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CourseId { get; set; }

        public virtual Course Course { get; set; }

        public Category(string name)
        {
            Name = name;
        }
    }
}
