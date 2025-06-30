using CentroTreinamento.Domain.Entities;
using CentroTreinamento.Domain.Repositories;
using CentroTreinamento.Infrastructure.Data;
// using Microsoft.EntityFrameworkCore; // Adicione se precisar de métodos específicos

namespace CentroTreinamento.Infrastructure.Repositories
{
    public class AdministradorRepository : Repository<Administrador>, IAdministradorRepository
    {
        public AdministradorRepository(ApplicationDbContext context) : base(context)
        {
        }
        // Se IAdministradorRepository tiver métodos específicos, implemente-os aqui.
    }
}