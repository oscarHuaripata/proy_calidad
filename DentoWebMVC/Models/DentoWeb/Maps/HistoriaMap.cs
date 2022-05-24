using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DentoWebMVC.Models.DentoWeb.Maps
{
    public class HistoriaMap : IEntityTypeConfiguration<Historia>
    {
        public void Configure(EntityTypeBuilder<Historia> builder)
        {
            builder.ToTable("Historia");
            builder.HasKey(o => o.idHistoria);

            builder.HasOne(o => o.cita).WithMany().HasForeignKey(o => o.idCita);
        }
    }
}
