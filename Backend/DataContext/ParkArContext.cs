using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Service.Enums;
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
        public DbSet<Configuracion> Configuraciones { get; set; }


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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Datos Semilla
            // =============================
            // CONFIGURACIONES
            // =============================
            modelBuilder.Entity<Configuracion>().HasData(
                new Configuracion{Id = 1, NombreEmpresa = "ParkAR", Direccion = "Av. Siempre Viva 123", Telefono = "341-555-1234", Email = "contacto@parkar.com", Cuit = "30-12345678-9", HorarioApertura = new TimeSpan(8, 0, 0), HorarioCierre = new TimeSpan(22, 0, 0), PrecioHora = 150.00m, IsDeleted = false}
            );

            // =============================
            // USUARIOS
            // =============================
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario { Id = 1, Nombre = "Juan Pérez", Email = "juan@example.com", Password = "hashed123", TipoUsuario = TipoUsuarioEnum.Cliente, IsDeleted = false },
                new Usuario { Id = 2, Nombre = "Admin Root", Email = "admin@example.com", Password = "hashed456", TipoUsuario = TipoUsuarioEnum.Admin, IsDeleted = false }
            );

            // =============================
            // VEHÍCULOS
            // =============================
            modelBuilder.Entity<Vehiculo>().HasData(
                new Vehiculo { Id = 1, Patente = "ABC123", TipoVehiculo = TipoVehiculoEnum.Auto, UsuarioId = 1, IsDeleted = false },
                new Vehiculo { Id = 2, Patente = "XYZ789", TipoVehiculo = TipoVehiculoEnum.Moto, UsuarioId = 1, IsDeleted = false }
            );

            // =============================
            // LUGARES
            // =============================
            modelBuilder.Entity<Lugar>().HasData(
                new Lugar { Id = 1, Numero = 101, IsDeleted = false },
                new Lugar { Id = 2, Numero = 102, IsDeleted = false }
            );

            // =============================
            // PLANES
            // =============================
            modelBuilder.Entity<Plan>().HasData(
                new Plan { Id = 1, Nombre = "Plan Mensual", Descripcion = "Acceso ilimitado por un mes", Precio = 5000, Duracion = 30, IsDeleted = false },
                new Plan { Id = 2, Nombre = "Plan por Hora", Descripcion = "Pago por cada hora de uso", Precio = 200, Duracion = null, IsDeleted = false }
            );

            // =============================
            // SUSCRIPCIONES
            // =============================
            modelBuilder.Entity<Suscripcion>().HasData(
                new Suscripcion { Id = 1, UsuarioId = 1, PlanId = 1, FechaInicio = DateTime.Parse("2025-08-01"), FechaFin = DateTime.Parse("2025-08-31"), Estado = EstadoSuscripcionEnum.Activo, IsDeleted = false },
                new Suscripcion { Id = 2, UsuarioId = 1, PlanId = 2, FechaInicio = DateTime.Parse("2025-08-20"), FechaFin = DateTime.Parse("2025-08-20"), Estado = EstadoSuscripcionEnum.Vencido, IsDeleted = false }
            );

            // =============================
            // RESERVAS
            // =============================
            modelBuilder.Entity<Reserva>().HasData(
                new Reserva { Id = 1, UsuarioId = 1, VehiculoId = 1, LugarId = 1, FechaInicio = DateTime.Parse("2025-08-25 09:00"), FechaFin = DateTime.Parse("2025-08-25 12:00"), EstadoReserva = EstadoReservaEnum.Finalizada, IsDeleted = false },
                new Reserva { Id = 2, UsuarioId = 1, VehiculoId = 2, LugarId = 2, FechaInicio = DateTime.Parse("2025-08-29 15:00"), FechaFin = DateTime.Parse("2025-08-29 18:00"), EstadoReserva = EstadoReservaEnum.Activa, IsDeleted = false }
            );

            // =============================
            // PAGOS
            // =============================
            modelBuilder.Entity<Pago>().HasData(
                new Pago { Id = 1, UsuarioId = 1, ReservaId = 1, SuscripcionId = null, Monto = 600.00m, Metodo = MetodoPagoEnum.Tarjeta, Fecha = DateTime.Parse("2025-08-25 12:05"), Concepto = ConceptoPagoEnum.Reserva, IsDeleted = false },
                new Pago { Id = 2, UsuarioId = 1, ReservaId = null, SuscripcionId = 1, Monto = 5000.00m, Metodo = MetodoPagoEnum.App, Fecha = DateTime.Parse("2025-08-01 08:00"), Concepto = ConceptoPagoEnum.Suscripcion, IsDeleted = false }
            );
            #endregion

            //configuramos los query filters para el borrado lógico
            modelBuilder.Entity<Usuario>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<Vehiculo>().HasQueryFilter(v => !v.IsDeleted);
            modelBuilder.Entity<Lugar>().HasQueryFilter(l => !l.IsDeleted);
            modelBuilder.Entity<Plan>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Suscripcion>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Reserva>().HasQueryFilter(r => !r.IsDeleted);
            modelBuilder.Entity<Pago>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Configuracion>().HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
