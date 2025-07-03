using CentroTreinamento.Domain.Repositories; // Para IRepository<TEntity>
using CentroTreinamento.Infrastructure.Data; // Para ApplicationDbContext
using Microsoft.EntityFrameworkCore; // Para DbSet e métodos assíncronos do EF Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CentroTreinamento.Infrastructure.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext _context; // O contexto do banco de dados
        protected readonly DbSet<TEntity> _dbSet; // A coleção DbSet para a entidade TEntity

        // O construtor recebe o DbContext via injeção de dependência
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>(); // Obtém o DbSet correto para a TEntity
        }

        public async Task<TEntity?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id); // Usa FindAsync do EF Core (busca por chave primária)
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync(); // Retorna todos os registros da tabela
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            // Permite consultas com lambda expressions (ex: a => a.Nome == "João")
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity); // Adiciona a entidade ao contexto, ela será salva depois
            //await _context.SaveChangesAsync(); // Salva as mudanças imediatamente (opcional, dependendo do padrão de uso)
        }

        // IMPLEMENTAÇÃO DE UpdateAsync
        public void UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity); // Marca a entidade como modificada
            //await _context.SaveChangesAsync(); // Salva as mudanças
        }

        // IMPLEMENTAÇÃO DE DeleteAsync
        public void DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity); // Marca a entidade para remoção
            //await _context.SaveChangesAsync(); // Salva as mudanças
        }

        public void Update(TEntity entity)
        {
            // Marca a entidade como modificada, ela será atualizada depois
            _dbSet.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            // Marca a entidade para ser removida, ela será deletada depois
            _dbSet.Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            // Este método é crucial: ele salva TODAS as mudanças rastreadas
            // (adições, atualizações, deleções) para o banco de dados.
            // É parte do padrão Unit of Work.
            return await _context.SaveChangesAsync();
        }
    }
}