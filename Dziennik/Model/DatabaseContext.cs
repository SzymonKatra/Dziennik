using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace Dziennik.Model
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(string filePath)
            : base(@"Data Source="+filePath)
        {
            Console.WriteLine("tworzenie");
            Database.SetInitializer<DatabaseContext>(new DropCreateDatabaseIfModelChanges<DatabaseContext>());
        }

        public DbSet<GlobalStudent> GlobalStudents { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<SchoolClass> SchoolClasses { get; set; }
        public DbSet<SchoolGroup> SchoolGroups { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<StudentInGroup> StudentsInGroups { get; set; }
    }
}
