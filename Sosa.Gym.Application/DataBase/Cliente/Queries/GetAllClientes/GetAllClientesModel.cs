namespace Sosa.Gym.Application.DataBase.Cliente.Queries.GetAllClientes
{
    public class GetAllClientesModel
    {
        // User Identity

        public int ClienteId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public long Dni { get; set; }
        public string Email { get; set; }
        public int Edad { get; set; }
        public decimal Altura { get; set; }
        public decimal Peso { get; set; }
        public string? Objetivo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
