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

            // crea un anagarifica
            var giorgio = new anagrafica() { Nome = "Giorgio", Cognome = "Saad" };
            context.Add(giorgio);

            // crea una lista di gruppi
            var amministratore = new gruppo() { Nome = "admin" };
            var utente = new gruppo() { Nome = "user" };

            // aggiungo i gruppi
            context.AddRange(amministratore, utente);

            // associo gli utenti ai gruppi
            amministratore.Utenti.Add(giorgio);

            // salva
            context.SaveChanges();

            var query = context.anagrafiche.Include(i => i.Gruppi);

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
    public class anagrafica : @base
    {
        public String Nome { get; set; }
        public String Cognome { get; set; }
        public List<gruppo> Gruppi { get; } = new List<gruppo>();
    }

    public class gruppo : @base
    {
        public String Nome { get; set; }
        public List<anagrafica> Utenti { get; } = new List<anagrafica>();
    }
    public class @base
    {
        public int id { get; set; }
        public String UtenteCreazione { get; set; }
        public DateTime DtCrezione { get; set; }
        public String UtenteModifica { get; set; }
        public DateTime? DtModifica { get; set; }
    }

    public class testDbContext : DbContext
    {
        public DbSet<anagrafica> anagrafiche { get; set; }
        public DbSet<gruppo> gruppi { get; set; }
        public override int SaveChanges()
        {
            var utenteCorrente = "giorgio160586";
            var l = ChangeTracker.Entries().Where(x => x.Entity is @base && 
                        (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var item in l)
            {
                if (item.State == EntityState.Added)
                {
                    ((@base)item.Entity).DtCrezione = DateTime.Now;
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
