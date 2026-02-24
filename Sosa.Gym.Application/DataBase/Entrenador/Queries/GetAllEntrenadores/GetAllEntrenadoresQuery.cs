using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sosa.Gym.Application.Features;
using Sosa.Gym.Domain.Models;

namespace Sosa.Gym.Application.DataBase.Entrenador.Queries.GetAllEntrenadores
{
    public class GetAllEntrenadoresQuery : IGetAllEntrenadoresQuery
    {
        private readonly IDataBaseService _db;

        public GetAllEntrenadoresQuery(IDataBaseService db)
        {
            _db = db;
        }

        public async Task<BaseResponseModel> Execute(int pageNumber, int pageSize, string? search)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            search = search?.Trim();

            var rolEntrenadorId = await _db.Roles
                .Where(r => r.Name == "Entrenador")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            if (rolEntrenadorId == 0)
                return ResponseApiService.Response(StatusCodes.Status500InternalServerError, "Rol Entrenador no existe");

            // Usuarios que son Entrenador
            var query = from u in _db.Usuarios.AsNoTracking()
                        join ur in _db.UserRoles.AsNoTracking() on u.Id equals ur.UserId
                        where ur.RoleId == rolEntrenadorId
                        select u;

            // Search (Nombre/Apellido/Email)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLowerInvariant();
                query = query.Where(u =>
                    (u.Nombre ?? "").ToLower().Contains(s) ||
                    (u.Apellido ?? "").ToLower().Contains(s) ||
                    (u.Email ?? "").ToLower().Contains(s));
            }

            var total = await query.CountAsync();

            var data = await query
                .Include(x => x.Cliente)
                .OrderBy(u => u.Apellido)
                .ThenBy(u => u.Nombre)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new GetEntrenadorModel
                {
                    Id = u.Id,
                    Email = u.Email!,
                    Nombre = u.Nombre!,
                    Apellido = u.Apellido!,
                    Dni = u.Dni
                })
                .ToListAsync();

            return ResponseApiService.Response(StatusCodes.Status200OK, new
            {
                pageNumber,
                pageSize,
                total,
                items = data
            });
        }

    }
}
