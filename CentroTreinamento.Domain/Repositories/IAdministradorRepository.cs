using CentroTreinamento.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace CentroTreinamento.Domain.Repositories
{
    public interface IAdministradorRepository : IRepository<Administrador>
    {
        // Exemplo: Se precisar buscar Administrador por CPF ou outro identificador único
        // Task<Administrador?> GetByCpfAsync(string cpf);
    }
}