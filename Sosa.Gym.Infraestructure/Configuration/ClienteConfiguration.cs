using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sosa.Gym.Domain.Entidades.Cliente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sosa.Gym.Infraestructure.Configuration
{
    public class ClienteConfiguration
    {

        public ClienteConfiguration(EntityTypeBuilder<ClienteEntity> entityBuilder)
        {
            entityBuilder.ToTable("Clientes");
            entityBuilder.HasKey(x => x.Id);

            entityBuilder.Property(x => x.Edad).IsRequired();
            entityBuilder.Property(x => x.Peso).IsRequired();
            entityBuilder.Property(x => x.Altura).IsRequired();
            entityBuilder.Property(x => x.Objetivo).IsRequired(false);
            entityBuilder.Property(x => x.FechaRegistro).IsRequired();

            entityBuilder.HasOne(x => x.Usuario)
                .WithOne(x => x.Cliente)
                .HasForeignKey<ClienteEntity>(x => x.Id);

            entityBuilder.HasMany(x => x.Progresos)
                .WithOne(x => x.Cliente)
                .HasForeignKey(x => x.ClienteId);

            entityBuilder.HasMany(x => x.RutinasAsignadas)
                .WithOne(x => x.Cliente)
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
