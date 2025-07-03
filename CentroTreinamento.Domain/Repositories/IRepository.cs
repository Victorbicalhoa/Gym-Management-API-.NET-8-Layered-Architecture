using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CentroTreinamento.Domain.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        // métodos de leitura
        Task<TEntity?> GetByIdAsync(Guid id); // TEntity? indica que pode retornar nulo
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        // métodos de escrita
        Task AddAsync(TEntity entity);
        //Task UpdateAsync(TEntity entity); 
        //Task DeleteAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);

        // método para salvar as alterações no contexto (para Unit of Work)
        Task<int> SaveChangesAsync(); 
    }
}