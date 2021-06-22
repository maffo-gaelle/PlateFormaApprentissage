using PlateFormaApprentissage.Models;
using PRBD_Framework;
using System;
using System.Linq;
using System.Windows;

namespace PlateFormaApprentissage
{

    public partial class App : ApplicationBase
    {

        public static Context context { get => Context<Context>(); }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.SeedData();
            // affichage du nombre d'instances de l'entité 'Member'
            Console.WriteLine(context.Users.Count());
        }
        protected override void OnRefreshData()
        {
            // pour plus tard
        }
    }
}
