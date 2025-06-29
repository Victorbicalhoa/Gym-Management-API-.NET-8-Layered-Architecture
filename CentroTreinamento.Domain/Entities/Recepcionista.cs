using System; // Para DateTime

public class Recepcionista
{
    // Atributos (Propriedades com get privados para controle)
    public int Id { get; private set; } // Identificador único da recepcionista
    public string Nome { get; private set; } // Nome completo da recepcionista
    public string SenhaHash { get; private set; } // Hash da senha para segurança
    public string Status { get; private set; } // Estado da recepcionista (e.g., "Ativo", "Inativo", "Ferias")

    // Construtor
    public Recepcionista(int id, string nome, string senhaHash, string status)
    {
        // Validações básicas no construtor para garantir que a entidade seja criada em um estado válido.
        if (id <= 0)
        {
            throw new ArgumentException("ID da recepcionista deve ser positivo.");
        }
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("Nome da recepcionista não pode ser vazio.");
        }
        if (string.IsNullOrWhiteSpace(senhaHash))
        {
            throw new ArgumentException("Senha hash da recepcionista não pode ser vazia.");
        }
        if (string.IsNullOrWhiteSpace(status))
        {
            throw new ArgumentException("Status da recepcionista não pode ser vazio.");
        }

        // Exemplo de validação de status inicial. Em um sistema real, usaria um Enum.
        if (status != "Ativo" && status != "Inativo" && status != "Ferias")
        {
            throw new ArgumentException($"Status '{status}' inválido para recepcionista na criação.");
        }

        Id = id;
        Nome = nome;
        SenhaHash = senhaHash;
        Status = status;
    }

    // Métodos Internos da Entidade (Comportamento de domínio)

    /// <summary>
    /// Retorna o status atual da recepcionista.
    /// </summary>
    /// <returns>O status da recepcionista (e.g., "Ativo", "Inativo", "Ferias").</returns>
    public string GetStatus()
    {
        return this.Status;
    }

    /// <summary>
    /// Atualiza o status da recepcionista.
    /// Deve validar se o novo status é válido para o domínio.
    /// </summary>
    /// <param name="novoStatus">O novo status a ser definido (e.g., "Ativo", "Inativo", "Ferias").</param>
    /// <exception cref="ArgumentException">Lançada se o novo status for inválido.</exception>
    public void AtualizarStatus(string novoStatus)
    {
        if (string.IsNullOrWhiteSpace(novoStatus))
        {
            throw new ArgumentException("Novo status não pode ser vazio.");
        }
        // Exemplo de validação de estado. Em um sistema real, usaria um Enum ou uma lista de constantes.
        if (novoStatus != "Ativo" && novoStatus != "Inativo" && novoStatus != "Ferias")
        {
            throw new ArgumentException($"Status '{novoStatus}' inválido para recepcionista.");
        }
        this.Status = novoStatus;
        // Lógica adicional pode ser adicionada aqui, como registrar um evento de domínio (Domain Event).
    }

    // Nota sobre 'SenhaHash': O setter é privado.
    // A SenhaHash deve ser definida apenas no construtor ou por um método específico
    // que lide com a redefinição de senha de forma segura.

    /// <summary>
    /// Retorna uma representação em string do objeto Recepcionista.
    /// </summary>
    /// <returns>Uma string que representa a recepcionista.</returns>
    public override string ToString()
    {
        return $"Recepcionista{{ Id={Id}, Nome='{Nome}', Status='{Status}' }}";
    }

    // Os "Serviços" listados (Cadastro de alunos, Gestão de agendamentos, Registro de pagamentos)
    // NÃO são métodos desta classe de entidade. Eles pertencem a classes de SERVIÇO (ex: UsuarioService,
    // AgendamentoService, PagamentoService) que interagem COM objetos Recepcionista (e outras entidades/repositórios)
    // para orquestrar as operações de negócio.
}