// CentroTreinamento.Domain/Repositories/IAdministradorRepository.cs
using CentroTreinamento.Domain.Entities;
using System.Threading.Tasks; // Pode ser necessário se adicionar métodos específicos

namespace CentroTreinamento.Domain.Repositories
{
    public interface IAdministradorRepository : IRepository<Administrador>
    {
        // Se você precisar de um método específico para buscar administrador por CPF,
        // e não quiser usar o FindAsync(a => a.Cpf == cpf) na camada de serviço,
        // você poderia adicionar algo como:
        // Task<Administrador?> GetByCpfAsync(string cpf);
        // Por enquanto, vamos manter simples e usar FindAsync no AppService.
    }
}