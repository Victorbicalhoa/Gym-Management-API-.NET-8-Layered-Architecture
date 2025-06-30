using CentroTreinamento.Domain.Entities; // Para acessar suas entidades de domínio
using Microsoft.EntityFrameworkCore; // Pacote do EF Core

namespace CentroTreinamento.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext // Herda de DbContext
    {
        // Construtor que recebe as opções do DbContext (para injeção de dependência)
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet para cada Entidade de Domínio que você quer mapear para uma tabela no banco
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Agendamento> Agendamentos { get; set; }
        public DbSet<PlanoDeTreino> PlanosDeTreino { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Instrutor> Instrutores { get; set; }
        public DbSet<Recepcionista> Recepcionistas { get; set; }

        // Opcional: Configurações de mapeamento avançadas
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Exemplo: Se suas entidades não herdam de BaseEntity com Id,
            // ou se o Id não é do tipo Guid por padrão, você pode mapear explicitamente:
            // modelBuilder.Entity<Aluno>().HasKey(a => a.Id);

            // Configurações para chaves estrangeiras, índices únicos, etc.
            // Ex: Garantir que o CPF do Aluno seja único
            // modelBuilder.Entity<Aluno>().HasIndex(a => a.Cpf).IsUnique();

            // Para suas entidades, garanta que o EF Core saiba a chave primária
            // Se você tem uma 'BaseEntity' com 'Id', o EF Core geralmente detecta automaticamente.
        }
    }
}