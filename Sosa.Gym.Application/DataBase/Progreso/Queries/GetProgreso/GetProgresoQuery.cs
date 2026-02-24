using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sosa.Gym.Application.DataBase.Ejercicio.Queries.GetEjerciciosByDiaRutina;
using Sosa.Gym.Application.Features;
using Sosa.Gym.Domain.Entidades.Progreso;
using Sosa.Gym.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sosa.Gym.Application.DataBase.Progreso.Queries.GetProgresoByCliente
{
    public class GetProgresoQuery : IGetProgresoQuery
    {
        private readonly IMapper _mapper;
        private readonly IDataBaseService _dataBaseService;

        public GetProgresoQuery(
            IMapper mapper,
            IDataBaseService dataBaseService
            )
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel> Execute(int userId)
        {
            var clienteId = await _dataBaseService.Clientes
                .Where(c => c.Id == userId)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            if (clienteId == 0)
                return ResponseApiService.Response(StatusCodes.Status404NotFound, "Cliente no encontrado");

            var progresos = await _dataBaseService.Progresos
                .Where(p => p.ClienteId == clienteId)
                .OrderByDescending(p => p.FechaRegistro)
                .ToListAsync();

            var data = _mapper.Map<List<GetProgresoModel>>(progresos);

            return ResponseApiService.Response(StatusCodes.Status200OK, data);
        }
    }
}
