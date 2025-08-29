using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Service.Models;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Emit;

namespace Backend.DataContext
{
    public class ParkARContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<Lugar> Lugares { get; set; }
        public DbSet<Plan> Planes { get; set; }
        public DbSet<Suscripcion> Suscripciones { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Pago> Pagos { get; set; }

        public ParkARContext() { }
        public ParkARContext(DbContextOptions<ParkARContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();
            }
        }

    }
}
