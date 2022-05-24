using DentoWebMVC.Models.DentoWeb;
using DentoWebMVC.Models.DentoWeb.Maps;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DentoWebMVC.Models.Context
{
    public class DentoWebContext : DbContext
    {
        public DentoWebContext(DbContextOptions<DentoWebContext> options)
        : base(options)
        {

        }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Historia> Historias { get; set; }
        public DbSet<Horario> Horarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ClienteMap());
            modelBuilder.ApplyConfiguration(new CitaMap());
            modelBuilder.ApplyConfiguration(new DoctorMap());
            modelBuilder.ApplyConfiguration(new HistoriaMap());
            modelBuilder.ApplyConfiguration(new HorarioMap());
        }
    }
}
