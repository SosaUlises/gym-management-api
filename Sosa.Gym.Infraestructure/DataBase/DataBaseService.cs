using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sosa.Gym.Application.DataBase;
using Sosa.Gym.Domain.Entidades.Cliente;
using Sosa.Gym.Domain.Entidades.Cuota;
using Sosa.Gym.Domain.Entidades.Ejercicio;
using Sosa.Gym.Domain.Entidades.Progreso;
using Sosa.Gym.Domain.Entidades.Rutina;
using Sosa.Gym.Domain.Entidades.Usuario;
using Sosa.Gym.Infraestructure.Configuration;

namespace Sosa.Gym.Infraestructure.DataBase
{
    public class DataBaseService : IdentityDbContext<UsuarioEntity, IdentityRole<int>, int>, IDataBaseService
    {
        public DataBaseService(DbContextOptions options) : base(options)
        {

        }

        public IQueryable<UsuarioEntity> Usuarios => Users.AsNoTracking();
        public IQueryable<IdentityRole<int>> Roles => base.Roles.AsNoTracking();
        public IQueryable<IdentityUserRole<int>> UserRoles => base.UserRoles.AsNoTracking();
        public DbSet<EjercicioEntity> Ejercicios { get; set; }
        public DbSet<ClienteEntity> Clientes { get; set; }
        public DbSet<ProgresoEntity> Progresos { get; set; }
        public DbSet<DiasRutinaEntity> DiasRutinas { get; set; }
        public DbSet<RutinaEntity> Rutinas { get; set; }
        public DbSet<CuotaEntity> Cuotas { get; set; }
        public DbSet<RutinaAsignadaEntity> RutinasAsignadas { get; set; }


        public async Task<bool> SaveAsync()
        {
            return await SaveChangesAsync() > 0;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            EntityConfiguration(modelBuilder);
        }

        private void EntityConfiguration(ModelBuilder modelBuilder)
        {
            new ClienteConfiguration(modelBuilder.Entity<ClienteEntity>());
            new RutinaConfiguration(modelBuilder.Entity<RutinaEntity>());
            new ProgresoConfiguration(modelBuilder.Entity<ProgresoEntity>());
            new EjercicioConfiguration(modelBuilder.Entity<EjercicioEntity>());
            new DiasRutinaConfiguration(modelBuilder.Entity<DiasRutinaEntity>());
            new CuotaConfiguration(modelBuilder.Entity<CuotaEntity>());
            new RutinaAsignadaConfiguration(modelBuilder.Entity<RutinaAsignadaEntity>());
        }
    }
}
