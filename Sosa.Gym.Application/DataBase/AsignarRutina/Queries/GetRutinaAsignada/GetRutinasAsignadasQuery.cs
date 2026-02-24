using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sosa.Gym.Application.Features;
using Sosa.Gym.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sosa.Gym.Application.DataBase.AsignarRutina.Queries.GetRutinaAsignada
{
    public class GetRutinasAsignadasQuery : IGetRutinasAsignadasQuery
    {
        private readonly IDataBaseService _db;
        private readonly IMapper _mapper;

        public GetRutinasAsignadasQuery(IDataBaseService db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel> Execute(int userId)
        {
            if (userId <= 0)
                return ResponseApiService.Response(StatusCodes.Status400BadRequest, "UserId inválido");

            var clienteId = await _db.Clientes
                .Where(c => c.Id == userId)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            if (clienteId == 0)
                return ResponseApiService.Response(StatusCodes.Status404NotFound, "Cliente no encontrado");

            var data = await _db.RutinasAsignadas
                .Where(x => x.ClienteId == clienteId)
                .OrderByDescending(x => x.FechaAsignacion)
                .ProjectTo<RutinaAsignadaItemModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return ResponseApiService.Response(StatusCodes.Status200OK, data);
        }
    }
}
