using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sosa.Gym.Application.Features;
using Sosa.Gym.Domain.Entidades.Cuota;
using Sosa.Gym.Domain.Models;
using System.Security.Claims;

namespace Sosa.Gym.Application.DataBase.Cuota.Queries.GetCuotasPendientes
{
    public class GetCuotasByEstadoQuery : IGetCuotasByEstadoQuery
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;

        public GetCuotasByEstadoQuery(
            IDataBaseService dataBaseService,
            IMapper mapper,
            IHttpContextAccessor http)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
            _http = http;
        }

        public async Task<BaseResponseModel> Execute(
             EstadoCuota estado,
             int userId,
             bool esAdmin)
        {
            IQueryable<CuotaEntity> query = _dataBaseService.Cuotas
                .Include(c => c.Cliente)
                .Where(c => c.Estado == estado);

            if (!esAdmin)
            {
                query = query.Where(c => c.Cliente.Id == userId);
            }

            var cuotas = await query
                .OrderByDescending(c => c.Anio)
                .ThenByDescending(c => c.Mes)
                .ToListAsync();

            if (!cuotas.Any())
            {
                return ResponseApiService.Response(
                    StatusCodes.Status404NotFound,
                    "No hay cuotas con ese estado");
            }

            return ResponseApiService.Response(
                StatusCodes.Status200OK,
                _mapper.Map<List<GetCuotasByEstadoModel>>(cuotas));
        }

    }
}
