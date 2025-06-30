using System; // Para ArgumentException

public class Administrador
{
    // Atributos (Propriedades com get privados para controle)
    public int Id { get; private set; } // Identificador único do administrador
    public string Nome { get; private set; } // Nome completo do administrador
    public string SenhaHash { get; private set; } // Hash da senha para segurança
    public string Status { get; private set; } // Estado do administrador (e.g., "Ativo", "Inativo")

    // Construtor
    public Administrador(int id, string nome, string senhaHash, string status)
    {
        // Validações básicas no construtor para garantir que a entidade seja criada em um estado válido.
        if (id <= 0)
        {
            throw new ArgumentException("ID do administrador deve ser positivo.");
        }
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("Nome do administrador não pode ser vazio.");
        }
        if (string.IsNullOrWhiteSpace(senhaHash))
        {
            throw new ArgumentException("Senha hash do administrador não pode ser vazia.");
        }
        if (string.IsNullOrWhiteSpace(status))
        {
            throw new ArgumentException("Status do administrador não pode ser vazio.");
        }

        // Exemplo de validação de status inicial. Em um sistema real, usaria um Enum.
        if (status != "Ativo" && status != "Inativo") // Os estados para Administrador são apenas Ativo/Inativo.
        {
            throw new ArgumentException($"Status '{status}' inválido para administrador na criação.");
        }

        Id = id;
        Nome = nome;
        SenhaHash = senhaHash;
        Status = status;
    }

    // Métodos Internos da Entidade (Comportamento de domínio)

    /// <summary>
    /// Retorna o status atual do administrador.
    /// </summary>
    /// <returns>O status do administrador (e.g., "Ativo", "Inativo").</returns>
    public string GetStatus()
    {
        return this.Status;
    }

    /// <summary>
    /// Atualiza o status do administrador.
    /// Deve validar se o novo status é válido para o domínio.
    /// </summary>
    /// <param name="novoStatus">O novo status a ser definido (e.g., "Ativo", "Inativo").</param>
    /// <exception cref="ArgumentException">Lançada se o novo status for inválido.</exception>
    public void AtualizarStatus(string novoStatus)
    {
        if (string.IsNullOrWhiteSpace(novoStatus))
        {
            throw new ArgumentException("Novo status não pode ser vazio.");
        }
        // Exemplo de validação de estado. Em um sistema real, usaria um Enum ou uma lista de constantes.
        if (novoStatus != "Ativo" && novoStatus != "Inativo")
        {
            throw new ArgumentException($"Status '{novoStatus}' inválido para administrador.");
        }
        this.Status = novoStatus;
        // Lógica adicional pode ser adicionada aqui, como registrar um evento de domínio (Domain Event).
    }

    // Nota sobre 'SenhaHash': O setter é privado
    // A SenhaHash deve ser definida apenas no construtor ou por um método específico
    // que lide com a redefinição de senha de forma segura.

    /// <summary>
    /// Retorna uma representação em string do objeto Administrador.
    /// </summary>
    /// <returns>Uma string que representa o administrador.</returns>
    public override string ToString()
    {
        return $"Administrador{{ Id={Id}, Nome='{Nome}', Status='{Status}' }}";
    }

    // Os "Serviços" listados (Gerenciamento de usuários, Auditoria financeira, Geração de relatórios, etc.)
    // NÃO são métodos desta classe de entidade. Eles pertencem a classes de SERVIÇO (ex: UsuarioService,
    // PagamentoService, RelatorioService, ConfigService) que interagem COM objetos Administrador
    // (e outras entidades/repositórios) para orquestrar as operações de negócio.
}