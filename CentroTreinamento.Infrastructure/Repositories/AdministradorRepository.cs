// CentroTreinamento.Infrastructure/Repositories/AdministradorRepository.cs
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Repositories;
using CentroTreinamento.Infrastructure.Data; // Para ApplicationDbContext

namespace CentroTreinamento.Infrastructure.Repositories
{
    // AdministradorRepository herda da implementação genérica e da sua interface específica
    public class AdministradorRepository : Repository<Administrador>, IAdministradorRepository
    {
        public AdministradorRepository(ApplicationDbContext context) : base(context)
        {
            // O construtor passa o DbContext para a classe base (Repository<TEntity>)
        }

        // Implemente aqui quaisquer métodos que você adicionou especificamente a IAdministradorRepository.
        // Se IAdministradorRepository estiver vazia (como no exemplo acima), esta classe também será vazia
        // além do construtor, pois herda tudo da base.
        /*
        // Exemplo de como implementar um método específico (se você tivesse GetByCpfAsync na interface):
        public async Task<Administrador?> GetByCpfAsync(string cpf)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Cpf == cpf);
        }
        */
    }
}