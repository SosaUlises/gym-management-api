using Microsoft.EntityFrameworkCore;
using Sosa.Gym.Domain.Entidades.Cliente;
using Sosa.Gym.Domain.Entidades.Cuota;
using Sosa.Gym.Domain.Entidades.Ejercicio;
using Sosa.Gym.Domain.Entidades.Progreso;
using Sosa.Gym.Domain.Entidades.Rutina;
using Sosa.Gym.Domain.Entidades.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sosa.Gym.Application.DataBase
{
    public interface IDataBaseService
    {
        DbSet<EjercicioEntity> Ejercicios { get; set; }
        DbSet<ProgresoEntity> Progresos { get; set; }
        DbSet<DiasRutinaEntity> DiasRutinas { get; set; }
        DbSet<RutinaEntity> Rutinas { get; set; }
        DbSet<ClienteEntity> Clientes { get; set; }
        DbSet<CuotaEntity> Cuotas { get; set; }
        DbSet<RutinaAsignadaEntity> RutinasAsignadas { get; set; }

        // Identity (solo lectura para queries)
        IQueryable<UsuarioEntity> Usuarios { get; }

        Task<bool> SaveAsync();
    }
}
