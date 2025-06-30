// CentroTreinamento.Infrastructure\Repositories\RecepcionistaRepository.cs
using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Repositories;
using CentroTreinamento.Infrastructure.Data;
// using Microsoft.EntityFrameworkCore; // Adicione se precisar de métodos específicos

namespace CentroTreinamento.Infrastructure.Repositories
{
    public class RecepcionistaRepository : Repository<Recepcionista>, IRecepcionistaRepository
    {
        public RecepcionistaRepository(ApplicationDbContext context) : base(context)
        {
        }
        // Se IRecepcionistaRepository tiver métodos específicos, implemente-os aqui.
    }
}