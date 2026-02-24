using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sosa.Gym.Domain.Entidades.Ejercicio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sosa.Gym.Infraestructure.Configuration
{
    public class EjercicioConfiguration
    {
        public EjercicioConfiguration(EntityTypeBuilder<EjercicioEntity> entityBuilder)
        {

            entityBuilder.ToTable("Ejercicios");
            entityBuilder.HasKey(x => x.Id);

            entityBuilder.Property(x => x.Nombre).IsRequired();
            entityBuilder.Property(x => x.PesoUtilizado).IsRequired();
            entityBuilder.Property(x => x.Repeticiones).IsRequired();
            entityBuilder.Property(x => x.Series).IsRequired();

            entityBuilder
            .HasOne(x => x.DiasRutina)
            .WithMany(x => x.Ejercicios)
            .HasForeignKey(x => x.DiaRutinaId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
