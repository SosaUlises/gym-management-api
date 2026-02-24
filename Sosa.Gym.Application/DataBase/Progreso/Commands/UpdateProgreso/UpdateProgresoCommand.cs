using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sosa.Gym.Application.DataBase.Ejercicio.Commands.UpdateEjercicio;
using Sosa.Gym.Application.Features;
using Sosa.Gym.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sosa.Gym.Application.DataBase.Progreso.Commands.UpdateProgreso
{
    public class UpdateProgresoCommand : IUpdateProgresoCommand
    {
        private readonly IMapper _mapper;
        private readonly IDataBaseService _dataBaseService;

        public UpdateProgresoCommand(
            IMapper mapper,
            IDataBaseService dataBaseService
            )
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel> Execute(int progresoId, UpdateProgresoModel model, int userId)
        {
            var progreso = await _dataBaseService.Progresos
                .FirstOrDefaultAsync(x => x.Id == progresoId);

            if (progreso == null)
                return ResponseApiService.Response(StatusCodes.Status404NotFound, "Progreso no encontrado");

            var clienteId = await _dataBaseService.Clientes
                .Where(c => c.Id == userId)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            if (clienteId == 0)
                return ResponseApiService.Response(StatusCodes.Status404NotFound, "Cliente no encontrado");

            if (progreso.ClienteId != clienteId)
            {
                return ResponseApiService.Response(
                    StatusCodes.Status403Forbidden,
                    "No puedes modificar progresos que no te pertenecen");
            }

            // Mapear solo editables
            progreso.PesoActual = model.PesoActual;
            progreso.Pecho = model.Pecho;
            progreso.Brazos = model.Brazos;
            progreso.Cintura = model.Cintura;
            progreso.Piernas = model.Piernas;
            progreso.Observaciones = model.Observaciones;

            await _dataBaseService.SaveAsync();

            return ResponseApiService.Response(StatusCodes.Status200OK, "Progreso actualizado correctamente");
        }
    }
}
