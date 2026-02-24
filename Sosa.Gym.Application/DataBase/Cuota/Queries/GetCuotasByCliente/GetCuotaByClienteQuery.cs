using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sosa.Gym.Application.DataBase.Cliente.Queries.GetClienteByDni;
using Sosa.Gym.Application.DataBase.Ejercicio.Queries.GetEjerciciosByDiaRutina;
using Sosa.Gym.Application.Features;
using Sosa.Gym.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sosa.Gym.Application.DataBase.Cuota.Queries.GetCuotaByCliente
{
    public class GetCuotaByClienteQuery : IGetCuotaByClienteQuery
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;
        public GetCuotaByClienteQuery(
            IDataBaseService dataBaseService,
            IMapper mapper

            )
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel> Execute(int clienteId, int userId, bool esAdmin)
        {
            var cliente = await _dataBaseService.Clientes
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Id == clienteId);

            if (cliente == null)
                return ResponseApiService.Response(StatusCodes.Status404NotFound, "Cliente no encontrado");

            if (!esAdmin && cliente.Id != userId)
            {
                return ResponseApiService.Response(
                    StatusCodes.Status403Forbidden,
                    "No puedes acceder a datos de otro usuario");
            }

            var cuotas = await _dataBaseService.Cuotas
                .Where(x => x.ClienteId == clienteId)
                .OrderByDescending(x => x.Anio)
                .ThenByDescending(x => x.Mes)
                .ToListAsync();

            var data = _mapper.Map<List<GetCuotaByClienteModel>>(cuotas);

            return ResponseApiService.Response(StatusCodes.Status200OK, data);
        }
    }
    }

