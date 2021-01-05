using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

            // crea una anagrafica
            var giorgio = new Anagrafica() { Nome = "Giorgio", Cognome = "Saad" };
            context.Add(giorgio);

            // crea una lista di gruppi
            var amministratore = new Gruppo() { Nome = "admin" };
            var utente = new Gruppo() { Nome = "user" };

            // aggiungo i gruppi
            context.AddRange(amministratore, utente);

            // associo gli utenti ai gruppi
            amministratore.Utenti.Add(giorgio);

            // salva
            context.SaveChanges();

            var query = context.Anagrafiche.Include(i => i.Gruppi);

            // mostra la query prossima all'esecuizione
            Console.WriteLine(query.ToQueryString());

            // mostra i risultati in console
            var l = query.ToList();

            foreach (var item in l)
            {
                Console.WriteLine($"ID: {item.id} Nome: {item.Nome} Cognome: {item.Cognome}");
            }

            // aggiorna il valore
            giorgio.Nome = "TEST";
            context.Update(giorgio);

            // salva nuovamente
            context.SaveChanges();
        }
    }
    public class Anagrafica : @base
    {
        public String Nome { get; set; }
        public String Cognome { get; set; }
        public List<Gruppo> Gruppi { get; } = new List<Gruppo>();
    }

    public class Gruppo : @base
    {
        public string Nome { get; set; }
        public List<Anagrafica> Utenti { get; } = new List<Anagrafica>();
    }
    public class @base
    {
        public int id { get; set; }
        public string UtenteCreazione { get; set; }
        public DateTime DtCreazione { get; set; }
        public string UtenteModifica { get; set; }
        public DateTime? DtModifica { get; set; }
    }

    public class testDbContext : DbContext
    {
        public DbSet<Anagrafica> Anagrafiche { get; set; }
        public DbSet<Gruppo> Gruppi { get; set; }
        public override int SaveChanges()
        {
            var utenteCorrente = "giorgio160586";
            var l = ChangeTracker.Entries().Where(x => x.Entity is @base && 
                        (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var item in l)
            {
                if (item.State == EntityState.Added)
                {
                    ((@base)item.Entity).DtCreazione = DateTime.Now;
                    ((@base)item.Entity).UtenteCreazione = utenteCorrente;
                }
                else
                {
                    ((@base)item.Entity).DtModifica = DateTime.Now;
                    ((@base)item.Entity).UtenteModifica = utenteCorrente;
                }
            }

            return base.SaveChanges();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
           => optionsBuilder
                    .LogTo(Console.WriteLine)
                    .EnableSensitiveDataLogging(true)
                    .UseSqlServer(@"Data Source=.\sqlexpress;Initial Catalog=EFCoreCRUDAppTest1;Integrated Security=True;");

    }
}
