using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sosa.Gym.Domain.Entidades.Progreso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sosa.Gym.Infraestructure.Configuration
{
    public class ProgresoConfiguration
    {
        public ProgresoConfiguration(EntityTypeBuilder<ProgresoEntity> entityBuilder)
        {
            entityBuilder.ToTable("Progresos");

            entityBuilder.HasKey(x => x.Id);
            entityBuilder.Property(x => x.PesoActual).IsRequired();
            entityBuilder.Property(x => x.Brazos).IsRequired();
            entityBuilder.Property(x => x.Cintura).IsRequired();
            entityBuilder.Property(x => x.FechaRegistro).IsRequired();
            entityBuilder.Property(x => x.Observaciones).IsRequired();
            entityBuilder.Property(x => x.Piernas).IsRequired();
            entityBuilder.Property(x => x.Pecho).IsRequired();

            entityBuilder.HasOne(x => x.Cliente).WithMany(x => x.Progresos).HasForeignKey(x => x.ClienteId);
        }
    }
}
