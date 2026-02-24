using Sosa.Gym.Domain.Models;

namespace Sosa.Gym.Application.DataBase.Entrenador.Queries.GetAllEntrenadores
{
    public interface IGetAllEntrenadoresQuery
    {
        Task<BaseResponseModel> Execute(int pageNumber, int pageSize, string? search);
    }
}
