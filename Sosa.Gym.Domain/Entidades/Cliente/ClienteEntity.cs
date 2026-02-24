using Sosa.Gym.Domain.Entidades.Cuota;
using Sosa.Gym.Domain.Entidades.Progreso;
using Sosa.Gym.Domain.Entidades.Rutina;
using Sosa.Gym.Domain.Entidades.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sosa.Gym.Domain.Entidades.Cliente
{
    public class ClienteEntity
    {
        public int Id { get; set; }
        public UsuarioEntity Usuario { get; set; }
        public int Edad { get; set; }
        public decimal Altura { get; set; }
        public decimal Peso { get; set; }
        public string? Objetivo { get; set; }
        public DateTime FechaRegistro { get; set; }

        public ICollection<ProgresoEntity> Progresos { get; set; } = new List<ProgresoEntity>();
        public ICollection<RutinaAsignadaEntity> RutinasAsignadas { get; set; } = new List<RutinaAsignadaEntity>();

        public ICollection<CuotaEntity> Cuotas { get; set; } = new List<CuotaEntity>();
    }
}
