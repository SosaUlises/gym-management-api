using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sosa.Gym.Application.Features;
using Sosa.Gym.Domain.Entidades.Usuario;
using Sosa.Gym.Domain.Models;

namespace Sosa.Gym.Application.DataBase.Cliente.Queries.GetClienteByDni
{
    public class GetClienteQuery : IGetClienteQuery
    {
        private readonly IMapper _mapper;
        private readonly IDataBaseService _dataBaseService;

        public GetClienteQuery(
            IDataBaseService dataBaseService,
            IMapper mapper
            )
        {
            _mapper = mapper;
            _dataBaseService = dataBaseService;
        }

        public async Task<BaseResponseModel> Execute(int userId)
        {
            var cliente = await _dataBaseService.Clientes
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Id == userId);

            if (cliente == null)
            {
                return ResponseApiService.Response(
                    StatusCodes.Status404NotFound,
                    "Cliente no encontrado");
            }

            return ResponseApiService.Response(
                StatusCodes.Status200OK,
                _mapper.Map<GetClienteModel>(cliente));
        }

    }
}
