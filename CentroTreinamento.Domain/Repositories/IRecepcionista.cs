using CentroTreinamento.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace CentroTreinamento.Domain.Repositories
{
    public interface IRecepcionistaRepository : IRepository<Recepcionista>
    {
        // Exemplo: Se precisar buscar Recepcionista por CPF ou outro identificador único
        // Task<Recepcionista?> GetByCpfAsync(string cpf);
    }
}