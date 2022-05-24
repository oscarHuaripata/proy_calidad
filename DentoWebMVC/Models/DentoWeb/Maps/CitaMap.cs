using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DentoWebMVC.Models.DentoWeb.Maps
{
    public class CitaMap : IEntityTypeConfiguration<Cita>
    {
        public void Configure(EntityTypeBuilder<Cita> builder)
        {
            builder.ToTable("Cita");
            builder.HasKey(o => o.idCita);
            builder.HasOne(o => o.doctor).WithMany(o => o.Citas).HasForeignKey(o => o.idDoctor);
            builder.HasOne(o => o.cliente).WithMany(o => o.Citas).HasForeignKey(o => o.idCliente);
            builder.HasOne(o => o.horario).WithMany().HasForeignKey(o => o.idHorario);
        }
    }
}
