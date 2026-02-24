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
    public class RutinaConfiguration
    {
        public RutinaConfiguration(EntityTypeBuilder<RutinaEntity> entityBuilder)
        {
            entityBuilder.ToTable("Rutinas");
            entityBuilder.HasKey(x => x.Id);

            entityBuilder.Property(x => x.Nombre).IsRequired();
            entityBuilder.Property(x => x.Descripcion).IsRequired();
            entityBuilder.Property(x => x.FechaCreacion).IsRequired();

            entityBuilder
                .HasMany(x => x.DiasRutina)
                .WithOne(x => x.Rutina)
                .HasForeignKey(x => x.RutinaId)
                .OnDelete(DeleteBehavior.Cascade);

            entityBuilder
                .HasMany(x => x.RutinasAsignadas)
                .WithOne(x => x.Rutina)
                .HasForeignKey(x => x.RutinaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
