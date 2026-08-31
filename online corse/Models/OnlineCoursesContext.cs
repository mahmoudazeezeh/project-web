using Microsoft.EntityFrameworkCore;

namespace online_corse.Models
{
    public class OnlineCoursesContext : DbContext
    {
        public OnlineCoursesContext(DbContextOptions<OnlineCoursesContext> options) : base(options)
        { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Corse> Corses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Corse>().HasData(
                new Corse { CorseId = 1, CorseName = "Web Design", Instructor = "Nour Saleh", CosrePrice = 120 },
                new Corse { CorseId = 2, CorseName = "Data Science", Instructor = "Rami Odeh", CosrePrice = 180 },
                new Corse { CorseId = 3, CorseName = "Digital Marketing", Instructor = "Sara Qasem", CosrePrice = 140 },
                new Corse { CorseId = 4, CorseName = "UI UX Studio", Instructor = "Lina Haddad", CosrePrice = 160 },
                new Corse { CorseId = 5, CorseName = "Python Basics", Instructor = "Omar Nasser", CosrePrice = 110 },
                new Corse { CorseId = 6, CorseName = "Graphic Design", Instructor = "Maya Faris", CosrePrice = 130 },
                new Corse { CorseId = 7, CorseName = "Business English", Instructor = "Huda Amin", CosrePrice = 90 },
                new Corse { CorseId = 8, CorseName = "Mobile Apps", Instructor = "Tarek Yousef", CosrePrice = 200 },
                new Corse { CorseId = 9, CorseName = "Cloud Intro", Instructor = "Farah Issa", CosrePrice = 150 },
                new Corse { CorseId = 10, CorseName = "Photography", Instructor = "Ziad Khatib", CosrePrice = 95 }
            );

            modelBuilder.Entity<Student>().HasData(
                new Student { StudentId = 1, Name = "Azeez Admin", Email = "aaazeezeh@gmail.com", Average = 100, Password = "Admin123", CorseId = 1 },
                new Student { StudentId = 2, Name = "Lina Haddad", Email = "lina@gmail.com", Average = 91, Password = "1234", CorseId = 4 },
                new Student { StudentId = 3, Name = "Omar Nasser", Email = "omar@gmail.com", Average = 84, Password = "1234", CorseId = 5 },
                new Student { StudentId = 4, Name = "Maya Faris", Email = "maya@gmail.com", Average = 88, Password = "1234", CorseId = 6 },
                new Student { StudentId = 5, Name = "Tarek Yousef", Email = "tarek@gmail.com", Average = 76, Password = "1234", CorseId = 8 },
                new Student { StudentId = 6, Name = "Huda Amin", Email = "huda@gmail.com", Average = 93, Password = "1234", CorseId = 7 },
                new Student { StudentId = 7, Name = "Ziad Khatib", Email = "ziad@gmail.com", Average = 79, Password = "1234", CorseId = 10 },
                new Student { StudentId = 8, Name = "Farah Issa", Email = "farah@gmail.com", Average = 86, Password = "1234", CorseId = 9 }
            );
        }
    }
}
