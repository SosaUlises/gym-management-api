using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sosa.Gym.Application.Features;
using Sosa.Gym.Domain.Entidades.Progreso;
using Sosa.Gym.Domain.Models;

namespace Sosa.Gym.Application.DataBase.Progreso.Commands.CreateProgreso
{
    public class CreateProgresoCommand : ICreateProgresoCommand
    {
        private readonly IMapper _mapper;
        private readonly IDataBaseService _dataBaseService;

        public CreateProgresoCommand(
            IMapper mapper,
            IDataBaseService dataBaseService
            )
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel> Execute(CreateProgresoModel model, int userId)
        {
            var clienteId = await _dataBaseService.Clientes
                .Where(c => c.Id == userId)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            if (clienteId == 0)
                return ResponseApiService.Response(StatusCodes.Status404NotFound, "Cliente no encontrado");

            var progreso = _mapper.Map<ProgresoEntity>(model);
            progreso.ClienteId = clienteId;
            progreso.FechaRegistro = DateTime.UtcNow;

            await _dataBaseService.Progresos.AddAsync(progreso);
            await _dataBaseService.SaveAsync();

            return ResponseApiService.Response(StatusCodes.Status201Created, "Progreso creado correctamente");
        }
    }
}
