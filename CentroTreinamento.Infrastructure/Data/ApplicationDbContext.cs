using CentroTreinamento.Domain.Entities; 
using Microsoft.EntityFrameworkCore;

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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeamento para a entidade Aluno
            modelBuilder.Entity<Aluno>(entity =>
            {
                entity.HasKey(e => e.Id); // Garante que Id é a chave primária
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(255); // Exemplo de configuração de propriedade
                entity.Property(e => e.Cpf).IsRequired().HasMaxLength(11); // CPF deve ter 11 dígitos, sem pontos/traços
                entity.HasIndex(e => e.Cpf).IsUnique(); // Garante que o CPF é único no banco de dados e cria um índice para busca rápida

                // Adicione outras configurações de propriedade ou relacionamentos aqui
                // Exemplo para Enum StatusAluno
                entity.Property(e => e.Status)
                      .HasConversion<string>() // Armazena o enum como string no DB
                      .HasMaxLength(20); // Define o tamanho máximo da string

                // Exemplo para Enum UserRole
                entity.Property(e => e.Role)
                      .HasConversion<string>()
                      .HasMaxLength(20);
            });

            // Configuração para Administrador 
            modelBuilder.Entity<Administrador>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Cpf).IsRequired().HasMaxLength(11);
                entity.HasIndex(e => e.Cpf).IsUnique(); // Assegura CPF único para Administrador
                entity.Property(e => e.SenhaHash).IsRequired();

                // Mapeamento de Enum para string (se você tiver StatusAdministrador)
                // Se você tiver um enum StatusAdministrador, adicione:
                // entity.Property(e => e.Status)
                //       .HasConversion<string>()
                //       .HasMaxLength(20);

                // Mapeamento de UserRole para string (se o Administrador tiver uma Role)
                // entity.Property(e => e.Role)
                //       .HasConversion<string>()
                //       .HasMaxLength(20);
            });

            // Configuração para Instrutor
            modelBuilder.Entity<Instrutor>(entity =>
            {
                entity.HasKey(e => e.Id); // Define Id como chave primária
                entity.HasIndex(e => e.Cpf).IsUnique(); // Garante que o CPF seja único
                entity.HasIndex(e => e.Cref).IsUnique(); // Garante que o CREF seja único
                // Outras configurações como tamanho máximo para strings, etc.
                entity.Property(e => e.Nome).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Cpf).HasMaxLength(11).IsRequired(); ; // CPF sem formatação
                entity.Property(e => e.SenhaHash).IsRequired();
                entity.Property(e => e.Cref).HasMaxLength(20).IsRequired();
                // O campo Role do Enum será persistido como int por padrão.
            });

            // Repita para outras entidades se necessário, como Agendamento, Administrador, etc.
            modelBuilder.Entity<Agendamento>().HasKey(a => a.Id);
            modelBuilder.Entity<PlanoDeTreino>().HasKey(p => p.Id);
            modelBuilder.Entity<Pagamento>().HasKey(p => p.Id);
            modelBuilder.Entity<Recepcionista>().HasKey(r => r.Id);

            // Configuração para a propriedade Valor da entidade Pagamento
            modelBuilder.Entity<Pagamento>()
                .Property(p => p.Valor)
                .HasColumnType("decimal(18, 2)");
        }
    }
}