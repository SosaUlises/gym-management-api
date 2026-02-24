using AutoMapper;
using Sosa.Gym.Application.DataBase.AsignarRutina.Queries.GetRutinaAsignada;
using Sosa.Gym.Application.DataBase.AsignarRutina.Queries.GetRutinaAsignadaDetalle;
using Sosa.Gym.Application.DataBase.Cliente.Commands.CreateCliente;
using Sosa.Gym.Application.DataBase.Cliente.Commands.UpdateCliente;
using Sosa.Gym.Application.DataBase.Cliente.Queries.GetAllClientes;
using Sosa.Gym.Application.DataBase.Cliente.Queries.GetClienteByDni;
using Sosa.Gym.Application.DataBase.Cuota.Commands.CreateCuota;
using Sosa.Gym.Application.DataBase.Cuota.Queries.GetCuotaByCliente;
using Sosa.Gym.Application.DataBase.Cuota.Queries.GetCuotasPendientes;
using Sosa.Gym.Application.DataBase.DiasRutina.Commands.CreateDiaRutina;
using Sosa.Gym.Application.DataBase.Ejercicio.Commands.CreateEjercicio;
using Sosa.Gym.Application.DataBase.Ejercicio.Queries.GetEjerciciosByDiaRutina;
using Sosa.Gym.Application.DataBase.Entrenador.Commands.CreateEntrenador;
using Sosa.Gym.Application.DataBase.Progreso.Commands.CreateProgreso;
using Sosa.Gym.Application.DataBase.Progreso.Commands.UpdateProgreso;
using Sosa.Gym.Application.DataBase.Progreso.Queries.GetProgresoByCliente;
using Sosa.Gym.Application.DataBase.Rutina.Commands.CreateRutina;
using Sosa.Gym.Application.DataBase.Rutina.Commands.UpdateRutina;
using Sosa.Gym.Application.DataBase.Rutina.Queries.GetRutinaDetalleAdmin;
using Sosa.Gym.Domain.Entidades.Cliente;
using Sosa.Gym.Domain.Entidades.Cuota;
using Sosa.Gym.Domain.Entidades.Ejercicio;
using Sosa.Gym.Domain.Entidades.Progreso;
using Sosa.Gym.Domain.Entidades.Rutina;
using Sosa.Gym.Domain.Entidades.Usuario;

namespace Sosa.Gym.Application.Configuration
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {


            // Clientes
            CreateMap<CreateClienteModel, UsuarioEntity>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<CreateClienteModel, ClienteEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario, opt => opt.Ignore())
                .ForMember(dest => dest.FechaRegistro, opt => opt.Ignore());

            CreateMap<UsuarioEntity, UpdateClienteModel>().ReverseMap();
            CreateMap<ClienteEntity, UpdateClienteModel>().ReverseMap();

            CreateMap<ClienteEntity, GetAllClientesModel>()
             .ForMember(dest => dest.ClienteId, opt => opt.MapFrom(src => src.Id))
             .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Usuario.Nombre))
             .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => src.Usuario.Apellido))
             .ForMember(dest => dest.Dni, opt => opt.MapFrom(src => src.Usuario.Dni))
             .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario.Email));


            CreateMap<ClienteEntity, GetClienteModel>()
               .ForMember(d => d.UsuarioId, o => o.MapFrom(s => s.Usuario.Id))
               .ForMember(d => d.Nombre, o => o.MapFrom(s => s.Usuario.Nombre))
               .ForMember(d => d.Apellido, o => o.MapFrom(s => s.Usuario.Apellido))
               .ForMember(d => d.Dni, o => o.MapFrom(s => s.Usuario.Dni))
               .ForMember(d => d.Email, o => o.MapFrom(s => s.Usuario.Email))
               .ForMember(d => d.ClienteId, o => o.MapFrom(s => s.Id));


            // Rutina
            CreateMap<CreateRutinaModel, RutinaEntity>()
                .ForMember(x => x.FechaCreacion, opt => opt.Ignore());

            CreateMap<RutinaEntity, UpdateRutinaModel>().ReverseMap();

            CreateMap<RutinaEntity, GetRutinaAdminDetalleModel>()
                .ForMember(dest => dest.Dias, opt => opt.MapFrom(src => src.DiasRutina));

            CreateMap<DiasRutinaEntity, GetDiaRutinaDetalleModel>();
            CreateMap<EjercicioEntity, GetEjercicioDetalleModel>();

            // DiaRutina
            CreateMap<CreateDiaRutinaModel, DiasRutinaEntity>()
                .ForMember(d => d.RutinaId, opt => opt.Ignore());

            // Ejercicio
            CreateMap<CreateEjercicioModel, EjercicioEntity>()
                .ForMember(e => e.DiaRutinaId, opt => opt.Ignore());
            CreateMap<EjercicioEntity, GetEjerciciosModel>().ReverseMap();

            // Progreso 
            CreateMap<CreateProgresoModel, ProgresoEntity>()
                .ForMember(x => x.ClienteId, opt => opt.Ignore())
                .ForMember(x => x.FechaRegistro, opt => opt.Ignore());
            CreateMap<UpdateProgresoModel, ProgresoEntity>()
               .ForMember(x => x.Id, opt => opt.Ignore())
               .ForMember(x => x.ClienteId, opt => opt.Ignore())
               .ForMember(x => x.FechaRegistro, opt => opt.Ignore());

            CreateMap<ProgresoEntity, GetProgresoModel>().ReverseMap();

            // Entrenador
            CreateMap<CreateEntrenadorModel, UsuarioEntity>();

            // Asignacion de rutinas

            CreateMap<RutinaAsignadaEntity, RutinaAsignadaItemModel>()
                .ForMember(d => d.RutinaId, o => o.MapFrom(s => s.RutinaId))
                .ForMember(d => d.Nombre, o => o.MapFrom(s => s.Rutina.Nombre))
                .ForMember(d => d.Descripcion, o => o.MapFrom(s => s.Rutina.Descripcion))
                .ForMember(d => d.FechaAsignacion, o => o.MapFrom(s => s.FechaAsignacion));

            CreateMap<RutinaEntity, RutinaAsignadaDetalleModel>()
                .ForMember(d => d.RutinaId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Dias, o => o.MapFrom(s => s.DiasRutina));

            CreateMap<DiasRutinaEntity, DiaRutinaDetalleModel>()
                .ForMember(d => d.DiaRutinaId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Ejercicios, o => o.MapFrom(s => s.Ejercicios));

            CreateMap<EjercicioEntity, EjercicioDetalleModel>()
                .ForMember(d => d.EjercicioId, o => o.MapFrom(s => s.Id));


            // Cuota
            CreateMap<CreateCuotaModel, CuotaEntity>()
                .ForMember(x => x.ClienteId, opt => opt.Ignore());
            CreateMap<CuotaEntity, GetCuotaByClienteModel>().ReverseMap();
            CreateMap<CuotaEntity, GetCuotasByEstadoModel>().ReverseMap();
        }
    }
}
