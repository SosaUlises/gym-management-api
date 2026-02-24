using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sosa.Gym.Application.Features;
using Sosa.Gym.Domain.Entidades.Usuario;
using Sosa.Gym.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sosa.Gym.Application.DataBase.Cliente.Commands.UpdateCliente
{
    public class UpdateClienteCommand : IUpdateClienteCommand
    {
        private readonly UserManager<UsuarioEntity> _userManager;
        private readonly IDataBaseService _dataBaseService;

        public UpdateClienteCommand(
            UserManager<UsuarioEntity> userManager,
            IDataBaseService dataBaseService)
        {
            _userManager = userManager;
            _dataBaseService = dataBaseService;
        }

        public async Task<BaseResponseModel> Execute(
            int clienteId,
            UpdateClienteModel model,
            int userIdLogueado,
            bool esAdmin)
        {

            var cliente = await _dataBaseService.Clientes
                .FirstOrDefaultAsync(c => c.Id == clienteId);

            if (cliente == null)
                return ResponseApiService.Response(StatusCodes.Status404NotFound, "Cliente no encontrado");


            if (!esAdmin && cliente.Id != userIdLogueado)
            {
                return ResponseApiService.Response(
                    StatusCodes.Status403Forbidden,
                    "No puedes acceder a datos de otro usuario");
            }


            var usuario = await _userManager.FindByIdAsync(cliente.Id.ToString());
            if (usuario == null)
                return ResponseApiService.Response(StatusCodes.Status404NotFound, "Usuario no encontrado");


            var existeEmail = await _userManager.Users
                .AnyAsync(x => x.Email == model.Email && x.Id != usuario.Id);

            if (existeEmail)
                return ResponseApiService.Response(
                    StatusCodes.Status400BadRequest,
                    $"Ya existe un usuario con el email {model.Email}");

            var existeDni = await _userManager.Users
                .AnyAsync(x => x.Dni == model.Dni && x.Id != usuario.Id);

            if (existeDni)
                return ResponseApiService.Response(
                    StatusCodes.Status400BadRequest,
                    $"Ya existe un usuario con el DNI {model.Dni}");


            usuario.Nombre = model.Nombre;
            usuario.Apellido = model.Apellido;
            usuario.Email = model.Email;
            usuario.UserName = model.Email;
            usuario.Dni = model.Dni;

            var updateUserResult = await _userManager.UpdateAsync(usuario);
            if (!updateUserResult.Succeeded)
                return ResponseApiService.Response(
                    StatusCodes.Status400BadRequest,
                    updateUserResult.Errors,
                    "Error al modificar el usuario");


            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
                var passResult = await _userManager.ResetPasswordAsync(usuario, token, model.Password);

                if (!passResult.Succeeded)
                    return ResponseApiService.Response(
                        StatusCodes.Status400BadRequest,
                        passResult.Errors,
                        "Error al modificar la contraseña");
            }


            if (esAdmin && !string.IsNullOrWhiteSpace(model.Rol))
            {
                var currentRoles = await _userManager.GetRolesAsync(usuario);
                var current = currentRoles.FirstOrDefault();

                if (!string.Equals(current, model.Rol, StringComparison.OrdinalIgnoreCase))
                {
                    if (current != null)
                    {
                        var removeRes = await _userManager.RemoveFromRoleAsync(usuario, current);
                        if (!removeRes.Succeeded)
                            return ResponseApiService.Response(
                                StatusCodes.Status400BadRequest,
                                removeRes.Errors,
                                "Error al actualizar roles");
                    }

                    var addRes = await _userManager.AddToRoleAsync(usuario, model.Rol);
                    if (!addRes.Succeeded)
                        return ResponseApiService.Response(
                            StatusCodes.Status400BadRequest,
                            addRes.Errors,
                            "Error al asignar rol");
                }
            }


            cliente.Edad = model.Edad;
            cliente.Altura = model.Altura;
            cliente.Peso = model.Peso;
            cliente.Objetivo = model.Objetivo;

            await _dataBaseService.SaveAsync();

            return ResponseApiService.Response(StatusCodes.Status200OK, "Cliente actualizado correctamente");
        }

        public async Task<BaseResponseModel> ExecuteMe(
        UpdateClienteModel model,
        int userIdLogueado)
        {
            var cliente = await _dataBaseService.Clientes
                .FirstOrDefaultAsync(c => c.Id == userIdLogueado);

            if (cliente == null)
            {
                return ResponseApiService.Response(
                    StatusCodes.Status404NotFound,
                    "Cliente no encontrado");
            }

            return await Execute(
                cliente.Id,
                model,
                userIdLogueado,
                esAdmin: false);
        }

    }

}
