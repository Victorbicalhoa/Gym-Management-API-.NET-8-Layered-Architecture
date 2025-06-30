using CentroTreinamento.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace CentroTreinamento.Domain.Repositories
{
    public interface IAlunoRepository : IRepository<Aluno>
    {
        // Se Aluno.CPF for um identificador único para busca, adicione:
        // Task<Aluno?> GetByCpfAsync(string cpf);
    }
}