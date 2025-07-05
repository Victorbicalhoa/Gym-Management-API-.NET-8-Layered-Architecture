// CentroTreinamento.Domain.Entities/Instrutor.cs
using System;
using CentroTreinamento.Domain.Enums;

namespace CentroTreinamento.Domain.Entities
{
    public class Instrutor
    {
        // Propriedades padrão para atores
        public Guid Id { get; private set; }
        public string? Nome { get; private set; }
        public string? Cpf { get; private set; }
        public string? SenhaHash { get; private set; }
        public StatusInstrutor Status { get; private set; }
        public string? Cref { get; private set; } // CREF é uma particularidade do Instrutor
        public UserRole Role { get; private set; }

        // Construtor vazio para o ORM (Entity Framework Core)
        public Instrutor() { }

        // Construtor completo com validações
        public Instrutor(Guid id, string nome, string? cpf, string senhaHash, StatusInstrutor status, string cref, UserRole role)
        {
            // Validação do Id aqui, já que não vem da BaseEntity
            if (id == Guid.Empty)
            {
                throw new ArgumentException("ID do instrutor não pode ser vazio.", nameof(id));
            }
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentException("Nome do instrutor não pode ser vazio.", nameof(nome));
            }
            if (string.IsNullOrWhiteSpace(senhaHash))
            {
                throw new ArgumentException("Senha hash do instrutor não pode ser vazia.", nameof(senhaHash));
            }
            if (string.IsNullOrWhiteSpace(cref))
            {
                throw new ArgumentException("CREF do instrutor não pode ser vazio.", nameof(cref));
            }
            // CPF pode ser null, mas se não for null, não pode ser vazio/whitespace
            if (cpf != null && string.IsNullOrWhiteSpace(cpf))
            {
                throw new ArgumentException("CPF do instrutor não pode ser vazio se não nulo.", nameof(cpf));
            }
            if (!Enum.IsDefined(typeof(StatusInstrutor), status))
            {
                throw new ArgumentException("Status inválido.", nameof(status));
            }
            // Força a Role para ser Instrutor na criação da entidade Instrutor
            if (role != UserRole.Instrutor)
            {
                throw new ArgumentException("Role inválida para instrutor. Deve ser 'Instrutor'.", nameof(role));
            }

            Id = id;
            Nome = nome;
            Cpf = cpf;
            SenhaHash = senhaHash;
            Status = status;
            Cref = cref;
            Role = role;
        }

        // --- Métodos Internos da Entidade (Comportamento de domínio) ---

        /// <summary>
        /// Atualiza o status do instrutor.
        /// </summary>
        /// <param name="novoStatus">O novo status a ser definido.</param>
        public void AtualizarStatus(StatusInstrutor novoStatus)
        {
            // Poderiam haver validações de transição de status aqui.
            // Validação básica para garantir que o status é um enum válido
            if (!Enum.IsDefined(typeof(StatusInstrutor), novoStatus))
            {
                throw new ArgumentException("Status de instrutor inválido.", nameof(novoStatus));
            }
            this.Status = novoStatus;
        }

        /// <summary>
        /// Atualiza dados gerais do instrutor.
        /// Os campos são atualizados apenas se os novos valores não forem nulos ou vazios/whitespace.
        /// </summary>
        /// <param name="novoNome">Novo nome do instrutor (opcional, se nulo/vazio, não altera).</param>
        /// <param name="novoCpf">Novo CPF do instrutor (opcional, se nulo/vazio, não altera).</param>
        /// <param name="novaSenhaHash">Novo hash da senha (opcional, se nulo/vazio, não altera).</param>
        /// <param name="novoCref">Novo CREF do instrutor (opcional, se nulo/vazio, não altera).</param>
        public void AtualizarDados(string? novoNome, string? novoCpf, string? novaSenhaHash, string? novoCref)
        {
            // Atualiza Nome apenas se o novoNome não for nulo, vazio ou conter apenas espaços em branco.
            if (!string.IsNullOrWhiteSpace(novoNome))
            {
                Nome = novoNome;
            }

            // Atualiza CPF apenas se o novoCpf não for nulo, vazio ou conter apenas espaços em branco.
            // Se string.Empty ou "   " for passado, isso significa que o CPF deve ser limpo/removido.
            // No entanto, se queremos manter o CPF existente, devemos verificar null/whitespace.
            // A validação no construtor indica que CPF não pode ser vazio se não nulo.
            // Aqui, se um novoCpf for fornecido (não null), ele deve ser válido.
            if (novoCpf != null) // Se for null, não faz nada (mantém o existente)
            {
                if (string.IsNullOrWhiteSpace(novoCpf))
                {
                    // Se o CPF é fornecido, mas é vazio/whitespace, significa que a intenção é setar como vazio,
                    // mas nossa regra de construtor não permite, então podemos decidir:
                    // 1. Manter o CPF existente (comportamento atual para null/empty/whitespace)
                    // 2. Lançar uma exceção (se CPF for obrigatório para atualização, mas nosso DTO permite null)
                    // 3. Permitir setar como null/vazio (se a regra de negócio mudar)
                    // Para consistência com as outras entidades (que não limpam campos opcionais se passados como vazios),
                    // e para evitar que um CPF válido se torne inválido acidentalmente, vamos manter a lógica de não atualizar
                    // se o valor for inválido, e o DTO deve ser validado antes de chegar aqui.
                    // Para o caso de inputModel.Cpf ser "", AppService já deve ter validado.
                    // Se a intenção é que CPF possa ser removido (setado para null), a chamada deveria passar explicitamente null.
                    // Se o DTO tem Cpf como string? e o frontend pode enviar "" para "remover", então é uma decisão.
                    // Por enquanto, vamos manter a regra: se não for nulo E não for válido, lança exceção.
                    throw new ArgumentException("CPF não pode ser vazio se fornecido.", nameof(novoCpf));
                }
                Cpf = novoCpf;
            }


            // Atualiza SenhaHash apenas se novaSenhaHash não for nula, vazia ou contiver apenas espaços em branco.
            if (!string.IsNullOrWhiteSpace(novaSenhaHash))
            {
                SenhaHash = novaSenhaHash;
            }

            // Atualiza CREF apenas se o novoCref não for nulo, vazio ou conter apenas espaços em branco.
            if (!string.IsNullOrWhiteSpace(novoCref))
            {
                Cref = novoCref;
            }
        }

        /// <summary>
        /// Define o hash da senha do instrutor.
        /// </summary>
        /// <param name="novaSenhaHash">O novo hash da senha.</param>
        public void SetSenhaHash(string novaSenhaHash)
        {
            if (string.IsNullOrWhiteSpace(novaSenhaHash))
            {
                throw new ArgumentException("Nova senha hash não pode ser vazia.", nameof(novaSenhaHash));
            }
            SenhaHash = novaSenhaHash;
        }

        /// <summary>
        /// Retorna uma representação em string do objeto Instrutor.
        /// </summary>
        /// <returns>Uma string que representa o instrutor.</returns>
        public override string ToString()
        {
            return $"Instrutor{{ Id={Id}, Nome='{Nome}', Status='{Status}', Cref='{Cref}' }}";
        }
    }
}