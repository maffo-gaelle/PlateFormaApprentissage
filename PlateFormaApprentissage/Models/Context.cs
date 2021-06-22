using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlateFormaApprentissage.Models
{
    public class Context : DbContextBase
    {
        public static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=gestionCours")
                          .EnableSensitiveDataLogging()
                          //.UseLoggerFactory(_loggerFactory)
                          .UseLazyLoadingProxies(true);
            //EF propose également une technique, appelée lazy loading, qui permet de charger automatiquement les données liées
            //au moment où la propriété de navigation correspondante est appelée. Pour l'activer, il faut ajouter la package NuGet
            //"Microsoft.EntityFrameworkCore.Proxies"
            //Pour que le lazy loading fonctionne, les propriétés de navigation doivent être marquées comme public et virtual .

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                
            });

            modelBuilder.Entity<Registration>(entity =>
            {
                entity.ToTable("Registration");

                entity.HasKey(Registration => new { Registration.CourseId, Registration.StudentId });

                entity.HasOne(r => r.User)
                      .WithMany(u => u.Registrations)
                      .HasForeignKey(r => r.StudentId)
                      .HasConstraintName("FK_registration_references_student")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Course)
                      .WithMany(c => c.Registrations)
                      .HasForeignKey(r => r.CourseId)
                      .HasConstraintName("FK_course_references_Registration")
                      .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Course");

                entity.HasOne(r => r.Professor)
                      .WithMany(u => u.Courses)
                      .HasForeignKey(r => r.ProfessorId)
                      .HasConstraintName("FK_course_references_User")
                      .OnDelete(DeleteBehavior.Restrict);
            });
            //modelBuilder.Entity<Vote>().HasKey(vote => new { vote.PostId, vote.UserId });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.HasOne(c => c.Course)
                      .WithMany(c => c.Categories)
                      .HasForeignKey(c => c.CourseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

        }

        public void SeedData()
        {
            if(Users.Count() == 0)
            { 
                Database.BeginTransaction();

                var aurelie = new User("aurelie", "lili", "lili@test.com", "lili", Role.Professor);
                var leaticia = new User("leaticia", "lealea", "lili@test.com", "lili", Role.Professor);
                var chris = new User("chris", "chrischris", "chris@test.com", "chris", Role.Student);
                var thomas = new User("thomas", "thomassson", "thomas@test.com", "thomas", Role.Student);

                Users.AddRange(new[] { aurelie, leaticia, chris, thomas });
                SaveChanges();

                var PRBD = new Course("PRBD", "Projet de développement", 10);
                var PRWB = new Course("PRWB", "Projet Web", 5);
                var ANC3 = new Course("ANC3", "Projet de conception", 6);
                var PRO2 = new Course("PRO2", "Projet Orienté Objet", 10);

                Courses.AddRange(new[] { PRBD, PRWB , ANC3 , PRO2 });

                aurelie.Courses.Add(PRBD);
                aurelie.Courses.Add(PRWB);
                leaticia.Courses.Add(ANC3);
                leaticia.Courses.Add(PRO2);

                var categories = new List<Category>()
                {
                    new Category("Informatique"),
                    new Category("Arithmétique"),
                    new Category("Mathematique"),
                    new Category("Test"),
                    new Category("JavaFx"),
                    new Category("Java"),
                    new Category("JSON"),
                    new Category("XML"),
                    new Category("CSS"),
                    new Category("JQuery")
                };

                Categories.AddRange(categories);

                PRBD.Categories.Add(categories[0]);
                PRBD.Categories.Add(categories[1]);
                PRBD.Categories.Add(categories[2]);
                PRBD.Categories.Add(categories[3]);
                PRBD.Categories.Add(categories[4]);
                ANC3.Categories.Add(categories[5]);
                ANC3.Categories.Add(categories[6]);
                ANC3.Categories.Add(categories[7]);
                ANC3.Categories.Add(categories[8]);
                PRO2.Categories.Add(categories[9]);

                SaveChanges();

                Database.CommitTransaction();

            }
        }
    }
}
