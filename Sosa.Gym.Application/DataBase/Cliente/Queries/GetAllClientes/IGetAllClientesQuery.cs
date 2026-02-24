using Sosa.Gym.Domain.Models;

namespace Sosa.Gym.Application.DataBase.Cliente.Queries.GetAllClientes
{
    public interface IGetAllClientesQuery
    {
        Task<BaseResponseModel> Execute(int pageNumber, int pageSize, string? search);
    }
}
