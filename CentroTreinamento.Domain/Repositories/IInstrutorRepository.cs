using CentroTreinamento.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentroTreinamento.Domain.Repositories
{
    public interface IInstrutorRepository : IRepository<Instrutor>
    {
        // Exemplo: Métodos específicos para Instrutor
        Task<IEnumerable<Instrutor>> GetInstrutoresDisponiveisAsync(DateTime data, TimeSpan horario);
        Task<Instrutor?> GetByCrefAsync(string cref); // Se CREF for um identificador único
    }
}