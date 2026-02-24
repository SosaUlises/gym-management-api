using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sosa.Gym.Application.Features;
using Sosa.Gym.Domain.Entidades.Cliente;
using Sosa.Gym.Domain.Entidades.Usuario;
using Sosa.Gym.Domain.Models;

namespace Sosa.Gym.Application.DataBase.Cliente.Commands.CreateCliente
{
    public class CreateClienteCommand : ICreateClienteCommand
    {
        private readonly UserManager<UsuarioEntity> _userManager;
        private readonly IMapper _mapper;
        private readonly IDataBaseService _dataBaseService;

        public CreateClienteCommand(
            UserManager<UsuarioEntity> userManager,
            IMapper mapper,
            IDataBaseService dataBaseService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _dataBaseService = dataBaseService;
        }

        public async Task<BaseResponseModel> Execute(CreateClienteModel model)
        {
            var email = model.Email?.Trim();

            if (string.IsNullOrWhiteSpace(email))
                return ResponseApiService.Response(StatusCodes.Status400BadRequest, "Email inválido");


            var usuarioExistente = await _userManager.FindByEmailAsync(email);
            if (usuarioExistente != null)
                return ResponseApiService.Response(StatusCodes.Status400BadRequest,
                    $"Ya existe un usuario con el email {email}");

            var existeDni = await _userManager.Users.AnyAsync(x => x.Dni == model.Dni);
            if (existeDni)
                return ResponseApiService.Response(StatusCodes.Status400BadRequest,
                    $"Ya existe un usuario con el DNI {model.Dni}");

            var usuario = _mapper.Map<UsuarioEntity>(model);
            usuario.UserName = email;
            usuario.Email = email;

            var createUser = await _userManager.CreateAsync(usuario, model.Password);
            if (!createUser.Succeeded)
                return ResponseApiService.Response(StatusCodes.Status400BadRequest, createUser.Errors, "Error al crear el usuario");

            const string rolCliente = "Cliente";
            var rolResult = await _userManager.AddToRoleAsync(usuario, rolCliente);
            if (!rolResult.Succeeded)
            {
                await _userManager.DeleteAsync(usuario);
                return ResponseApiService.Response(StatusCodes.Status400BadRequest, rolResult.Errors, "Error al asignar el rol");
            }

            try
            {
                var cliente = _mapper.Map<ClienteEntity>(model);
                cliente.Id = usuario.Id;
                cliente.FechaRegistro = DateTime.UtcNow;

                _dataBaseService.Clientes.Add(cliente);
                await _dataBaseService.SaveAsync();

                return ResponseApiService.Response(
                    StatusCodes.Status201Created,
                    new { UsuarioId = usuario.Id, ClienteId = cliente.Id, usuario.Email, usuario.Nombre, usuario.Apellido },
                    "Cliente creado correctamente");
            }
            catch
            {
                await _userManager.DeleteAsync(usuario);
                throw;
            }
        }
    
    
}

}

