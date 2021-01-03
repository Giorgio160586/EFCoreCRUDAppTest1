using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EFCoreCRUDAppTest1
{
    public class Program
    {
        static void Main(string[] args)
        {
            var context = new testDbContext();
            // Drop the database if it exists
            context.Database.EnsureDeleted();

            // Create the database if it doesn't exist
            context.Database.EnsureCreated();

            context.Add(new anagrafica() { Nome = "Giorgio", Cognome = "Saad" });
            context.SaveChanges();

            var l = context.anagrafiche.ToList();

            foreach (var item in l)
            {
                Console.WriteLine($"ID: {item.id} Nome: {item.Nome} Cognome: {item.Cognome}");
            }
        }
    }
    public class anagrafica
    {
        public int id { get; set; }
        public String Nome { get; set; }
        public String Cognome { get; set; }
    }

    public class testDbContext : DbContext
    {   
        public DbSet<anagrafica> anagrafiche { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=.\sqlexpress;Initial Catalog=EFCoreCRUDAppTest1;Integrated Security=True;");
        }
    }
}
