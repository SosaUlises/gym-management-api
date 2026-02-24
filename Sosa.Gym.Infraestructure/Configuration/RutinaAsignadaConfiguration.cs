using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sosa.Gym.Domain.Entidades.Rutina;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sosa.Gym.Infraestructure.Configuration
{
    public class RutinaAsignadaConfiguration
    {
        public RutinaAsignadaConfiguration(EntityTypeBuilder<RutinaAsignadaEntity> entityBuilder)
        {
            entityBuilder.ToTable("RutinasAsignadas");
            entityBuilder.HasKey(x => x.Id);

            entityBuilder.Property(x => x.FechaAsignacion).IsRequired();

            entityBuilder.HasIndex(x => new { x.ClienteId, x.RutinaId }).IsUnique();

            entityBuilder.HasOne(x => x.Cliente)
                .WithMany(c => c.RutinasAsignadas)
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            entityBuilder.HasOne(x => x.Rutina)
                .WithMany(r => r.RutinasAsignadas)
                .HasForeignKey(x => x.RutinaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
