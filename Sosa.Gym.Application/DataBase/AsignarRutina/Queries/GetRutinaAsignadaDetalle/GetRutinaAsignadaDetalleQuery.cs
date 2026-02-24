using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sosa.Gym.Application.Features;
using Sosa.Gym.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sosa.Gym.Application.DataBase.AsignarRutina.Queries.GetRutinaAsignadaDetalle
{
    public class GetRutinaAsignadaDetalleQuery : IGetRutinaAsignadaDetalleQuery
    {
        private readonly IDataBaseService _db;
        private readonly IMapper _mapper;

        public GetRutinaAsignadaDetalleQuery(IDataBaseService db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel> Execute(int rutinaId, int userId)
        {
            if (rutinaId <= 0 || userId <= 0)
                return ResponseApiService.Response(StatusCodes.Status400BadRequest, "Parámetros inválidos");

            var clienteId = await _db.Clientes
                .Where(c => c.Id == userId)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            if (clienteId == 0)
                return ResponseApiService.Response(StatusCodes.Status404NotFound, "Cliente no encontrado");

            var asignada = await _db.RutinasAsignadas
                .AnyAsync(x => x.ClienteId == clienteId && x.RutinaId == rutinaId);

            if (!asignada)
                return ResponseApiService.Response(StatusCodes.Status404NotFound, "Rutina no asignada");

            var rutina = await _db.Rutinas
                .Include(r => r.DiasRutina)
                    .ThenInclude(d => d.Ejercicios)
                .FirstOrDefaultAsync(r => r.Id == rutinaId);

            if (rutina == null)
                return ResponseApiService.Response(StatusCodes.Status404NotFound, "Rutina no encontrada");

            var dto = _mapper.Map<RutinaAsignadaDetalleModel>(rutina);

            dto.Dias = dto.Dias.OrderBy(d => d.DiaRutinaId).ToList();
            foreach (var dia in dto.Dias)
                dia.Ejercicios = dia.Ejercicios.OrderBy(e => e.EjercicioId).ToList();

            return ResponseApiService.Response(StatusCodes.Status200OK, dto);
        }
    }
}
