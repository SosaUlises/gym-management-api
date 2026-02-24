using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sosa.Gym.Application.Features;
using Sosa.Gym.Domain.Entidades.Usuario;
using Sosa.Gym.Domain.Models;

namespace Sosa.Gym.Application.DataBase.Cliente.Commands.DeleteCliente
{
    public class DeleteClienteCommand : IDeleteClienteCommand
    {

        private readonly IDataBaseService _dataBaseService;
        private readonly UserManager<UsuarioEntity> _userManager;
        public DeleteClienteCommand(
            UserManager<UsuarioEntity> userManager,
            IDataBaseService dataBaseService
            )
        {
            _dataBaseService = dataBaseService;
            _userManager = userManager;
        }

        public async Task<BaseResponseModel> Execute(int clienteId)
        {
            var cliente = await _dataBaseService.Clientes
                .FirstOrDefaultAsync(x => x.Id == clienteId);

            if (cliente == null)
                return ResponseApiService.Response(StatusCodes.Status404NotFound, "Cliente no encontrado");

            var user = await _userManager.FindByIdAsync(cliente.Id.ToString());
            if (user == null)
                return ResponseApiService.Response(StatusCodes.Status404NotFound, "Usuario no encontrado");

            _dataBaseService.Clientes.Remove(cliente);

            try
            {
                await _dataBaseService.SaveAsync();
            }
            catch
            {
                return ResponseApiService.Response(
                    StatusCodes.Status400BadRequest,
                    "Error al eliminar el cliente");
            }

            var resultUsuario = await _userManager.DeleteAsync(user);
            if (!resultUsuario.Succeeded)
            {
                return ResponseApiService.Response(
                    StatusCodes.Status400BadRequest,
                    "Cliente eliminado pero ocurrió un error al eliminar el usuario");
            }

            return ResponseApiService.Response(StatusCodes.Status200OK, "Cliente eliminado correctamente");
        }

    }
}
