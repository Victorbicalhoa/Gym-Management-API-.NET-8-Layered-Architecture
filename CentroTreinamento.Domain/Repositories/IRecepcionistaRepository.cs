// CentroTreinamento.Domain/Repositories/IRecepcionistaRepository.cs
using CentroTreinamento.Domain.Entities;
using System.Threading.Tasks;

namespace CentroTreinamento.Domain.Repositories
{
    public interface IRecepcionistaRepository : IRepository<Recepcionista>
    {
        // Métodos específicos para Recepcionista, se houver.
        // GetByCpfAsync é um bom exemplo.
        Task<Recepcionista?> GetByCpfAsync(string cpf);
    }
}