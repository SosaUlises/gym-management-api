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
    public class DiasRutinaConfiguration
    {
        public DiasRutinaConfiguration(EntityTypeBuilder<DiasRutinaEntity> entityBuilder)
        {

            entityBuilder.ToTable("DiasRutina");
            entityBuilder.HasKey(x => x.Id);

            entityBuilder.Property(x => x.NombreDia).IsRequired();


            entityBuilder
              .HasOne(x => x.Rutina)
              .WithMany(x => x.DiasRutina)
              .HasForeignKey(x => x.RutinaId)
              .OnDelete(DeleteBehavior.Cascade);

            entityBuilder
                .HasMany(x => x.Ejercicios)
                .WithOne(x => x.DiasRutina)
                .HasForeignKey(x => x.DiaRutinaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
