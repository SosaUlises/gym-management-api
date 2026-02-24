using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sosa.Gym.Application.DataBase.Cliente.Commands.CreateCliente;
using Sosa.Gym.Application.DataBase.Cliente.Commands.DeleteCliente;
using Sosa.Gym.Application.DataBase.Cliente.Commands.UpdateCliente;
using Sosa.Gym.Application.DataBase.Cliente.Queries.GetAllClientes;
using Sosa.Gym.Application.DataBase.Cliente.Queries.GetClienteAdmin;
using Sosa.Gym.Application.DataBase.Cliente.Queries.GetClienteByDni;
using Sosa.Gym.Application.Features;
using System.Security.Claims;

namespace Sosa.Gym.API.Controllers
{
    [Route("api/v1/clientes")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdStr, out userId);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateClienteModel model,
            [FromServices] ICreateClienteCommand createClienteCommand,
            [FromServices] IValidator<CreateClienteModel> validator)
        {
            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(ResponseApiService.Response(
                    StatusCodes.Status400BadRequest,
                    validationResult.Errors));
            }

            var result = await createClienteCommand.Execute(model);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe(
            [FromBody] UpdateClienteModel model,
            [FromServices] IUpdateClienteCommand updateClienteCommand,
            [FromServices] IValidator<UpdateClienteModel> validator)
        {
            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(ResponseApiService.Response(
                    StatusCodes.Status400BadRequest,
                    validationResult.Errors));
            }

            if (!TryGetUserId(out var userId))
            {
                return Unauthorized(ResponseApiService.Response(
                    StatusCodes.Status401Unauthorized,
                    "Token inválido"));
            }

            var result = await updateClienteCommand.ExecuteMe(model, userId);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{clienteId:int}")]
        public async Task<IActionResult> UpdateByAdmin(
            [FromRoute] int clienteId,
            [FromBody] UpdateClienteModel model,
            [FromServices] IUpdateClienteCommand updateClienteCommand,
            [FromServices] IValidator<UpdateClienteModel> validator)
        {
            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(ResponseApiService.Response(
                    StatusCodes.Status400BadRequest,
                    validationResult.Errors));
            }

            if (!TryGetUserId(out var userId))
            {
                return Unauthorized(ResponseApiService.Response(
                    StatusCodes.Status401Unauthorized,
                    "Token inválido"));
            }

            var result = await updateClienteCommand.Execute(clienteId, model, userId, esAdmin: true);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("{clienteId:int}")]
        public async Task<IActionResult> Delete(
            [FromRoute] int clienteId,
            [FromServices] IDeleteClienteCommand deleteClienteCommand)
        {
            if (clienteId <= 0)
            {
                return BadRequest(ResponseApiService.Response(
                    StatusCodes.Status400BadRequest,
                    "Id inválido"));
            }

            var result = await deleteClienteCommand.Execute(clienteId);
            return StatusCode(result.StatusCode, result);
        }


        [Authorize(Roles = "Administrador,Entrenador")]
        [HttpGet]
        public async Task<IActionResult> GetAll(
             [FromServices] IGetAllClientesQuery query,
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


        [Authorize] 
        [HttpGet("me")]
        public async Task<IActionResult> GetMe(
            [FromServices] IGetClienteQuery query)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized(ResponseApiService.Response(
                    StatusCodes.Status401Unauthorized,
                    "Token inválido"));
            }

            var result = await query.Execute(userId);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "Administrador,Entrenador")]
        [HttpGet("{clienteId:int}")]
        public async Task<IActionResult> GetById(
            [FromRoute] int clienteId,
            [FromServices] IGetClienteByIdAdminQuery query)
        {
            if (clienteId <= 0)
            {
                return BadRequest(ResponseApiService.Response(
                    StatusCodes.Status400BadRequest,
                    "Id inválido"));
            }

            var result = await query.Execute(clienteId);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "Administrador,Entrenador")]
        [HttpGet("dni/{dni:long}")]
        public async Task<IActionResult> GetByDni(
            [FromRoute] long dni,
            [FromServices] IGetClienteByDniQuery getClienteByDniQuery)
        {
            if (dni <= 0)
            {
                return BadRequest(ResponseApiService.Response(
                    StatusCodes.Status400BadRequest,
                    "DNI inválido"));
            }

            var result = await getClienteByDniQuery.Execute(dni);
            return StatusCode(result.StatusCode, result);
        }
    }
}
