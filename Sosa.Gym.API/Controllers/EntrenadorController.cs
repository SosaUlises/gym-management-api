using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sosa.Gym.Application.DataBase.Cliente.Queries.GetAllClientes;
using Sosa.Gym.Application.DataBase.Entrenador.Commands.CreateEntrenador;
using Sosa.Gym.Application.DataBase.Entrenador.Commands.DeleteEntrenador;
using Sosa.Gym.Application.DataBase.Entrenador.Commands.UpdateEntrenador;
using Sosa.Gym.Application.DataBase.Entrenador.Queries.GetAllEntrenadores;
using Sosa.Gym.Application.DataBase.Entrenador.Queries.GetEntrenadorByDni;
using Sosa.Gym.Application.DataBase.Entrenador.Queries.GetEntrenadorById;
using Sosa.Gym.Application.Features;

namespace Sosa.Gym.API.Controllers
{

    [Route("api/v1/entrenadores")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
    public class EntrenadorController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create(
        [FromBody] CreateEntrenadorModel model,
        [FromServices] ICreateEntrenadorCommand command,
        [FromServices] IValidator<CreateEntrenadorModel> validator)
        {
            var vr = await validator.ValidateAsync(model);
            if (!vr.IsValid) return BadRequest(ResponseApiService.Response(400, vr.Errors));

            var result = await command.Execute(model);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{userId:int}")]
        public async Task<IActionResult> Update(
            [FromRoute] int userId,
            [FromBody] UpdateEntrenadorModel model,
            [FromServices] IUpdateEntrenadorCommand command,
            [FromServices] IValidator<UpdateEntrenadorModel> validator)
        {
            if (userId <= 0) return BadRequest(ResponseApiService.Response(400, "Id inválido"));
            var vr = await validator.ValidateAsync(model);
            if (!vr.IsValid) return BadRequest(ResponseApiService.Response(400, vr.Errors));

            var result = await command.Execute(userId, model);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{userId:int}")]
        public async Task<IActionResult> Delete(
            [FromRoute] int userId,
            [FromServices] IDeleteEntrenadorCommand command)
        {
            if (userId <= 0) return BadRequest(ResponseApiService.Response(400, "Id inválido"));
            var result = await command.Execute(userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> GetAll(
             [FromServices] IGetAllEntrenadoresQuery query,
             [FromQuery] int pageNumber = 1,
             [FromQuery] int pageSize = 10,
             [FromQuery] string? search = null)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var result = await query.Execute(pageNumber, pageSize, search);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetById(
            [FromRoute] int userId,
            [FromServices] IGetEntrenadorByIdQuery query)
        {
            if (userId <= 0) return BadRequest(ResponseApiService.Response(400, "Id inválido"));
            var result = await query.Execute(userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("dni/{dni:long}")]
        public async Task<IActionResult> GetByDni(
            [FromRoute] long dni,
            [FromServices] IGetEntrenadorByDniQuery query)
        {
            if (dni <= 0) return BadRequest(ResponseApiService.Response(400, "Dni inválido"));
            var result = await query.Execute(dni);
            return StatusCode(result.StatusCode, result);
        }
    }
}
