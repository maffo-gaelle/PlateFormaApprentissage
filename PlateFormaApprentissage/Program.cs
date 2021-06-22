using PlateFormaApprentissage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlateFormaApprentissage
{
    public class Program
    {
        public static void main(string[] args)
        {
            //Crée une instance du du contexte. Le mot using permet d'indiquer que le contexte doit être disposed
            using var model = new Context();

            //pour se rassurer de supprimer la base de données avant de la recréer
            model.Database.EnsureDeleted();

            //Demande au contexte de garantir que la bas de données existe si ce n'est pas le cas EF la crée
            model.Database.EnsureCreated();

        }
    }
}
