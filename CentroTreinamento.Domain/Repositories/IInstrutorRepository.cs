using CentroTreinamento.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentroTreinamento.Domain.Repositories
{
    public interface IInstrutorRepository : IRepository<Instrutor>
    {
        Task<IEnumerable<Instrutor>> GetInstrutoresDisponiveisAsync(DateTime data, TimeSpan horario);
        Task<Instrutor?> GetByCrefAsync(string cref);
        Task<Instrutor?> GetByCpfAsync(string cpf);
    }
}