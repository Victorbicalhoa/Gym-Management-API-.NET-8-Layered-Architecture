using System; // Para DateTime

public class Instrutor
{
    // Atributos (Propriedades com get privados para controle)
    public int Id { get; private set; } // Identificador único do instrutor
    public string Nome { get; private set; } // Nome completo do instrutor
    public string SenhaHash { get; private set; } // Hash da senha para segurança
    public string Status { get; private set; } // Estado do instrutor (e.g., "Ativo", "Inativo", "Ferias")

    // Construtor
    public Instrutor(int id, string nome, string senhaHash, string status)
    {
        // Validações básicas no construtor para garantir que a entidade seja criada em um estado válido.
        if (id <= 0)
        {
            throw new ArgumentException("ID do instrutor deve ser positivo.");
        }
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("Nome do instrutor não pode ser vazio.");
        }
        if (string.IsNullOrWhiteSpace(senhaHash))
        {
            throw new ArgumentException("Senha hash do instrutor não pode ser vazia.");
        }
        if (string.IsNullOrWhiteSpace(status))
        {
            throw new ArgumentException("Status do instrutor não pode ser vazio.");
        }

        // Exemplo de validação de status inicial. Em um sistema real, usaria um Enum.
        if (status != "Ativo" && status != "Inativo" && status != "Ferias")
        {
            throw new ArgumentException($"Status '{status}' inválido para instrutor na criação.");
        }

        Id = id;
        Nome = nome;
        SenhaHash = senhaHash;
        Status = status;
    }

    // Métodos Internos da Entidade (Comportamento de domínio)

    /// <summary>
    /// Atualiza o status do instrutor.
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
            throw new ArgumentException($"Status '{novoStatus}' inválido para instrutor.");
        }
        this.Status = novoStatus;
        // Lógica adicional pode ser adicionada aqui, como registrar um evento de domínio (Domain Event).
    }

    // Nota sobre 'SenhaHash': O setter é privado.
    // A SenhaHash deve ser definida apenas no construtor ou por um método específico
    // que lide com a redefinição de senha de forma segura (ex: um método dentro de um Serviço de Autenticação/Usuário
    // que receba a nova senha em texto puro, faça o hash e chame um método interno seguro para atualização).

    // Exemplo de como um método para definir senha hash (mas a lógica de hashing estaria em outro lugar)
    // internal void SetSenhaHash(string novaSenhaHash) // 'internal' para acesso restrito dentro do mesmo assembly
    // {
    //     if (string.IsNullOrWhiteSpace(novaSenhaHash))
    //     {
    //         throw new ArgumentException("Nova senha hash não pode ser vazia.");
    //     }
    //     this.SenhaHash = novaSenhaHash;
    // }


    /// <summary>
    /// Retorna uma representação em string do objeto Instrutor.
    /// </summary>
    /// <returns>Uma string que representa o instrutor.</returns>
    public override string ToString()
    {
        return $"Instrutor{{ Id={Id}, Nome='{Nome}', Status='{Status}' }}";
    }

    // Os "Serviços" (criação/edição de planos, gestão de agenda, etc.)
    // NÃO são métodos desta classe de entidade. Eles pertencem a classes de SERVIÇO
    // que interagem COM objetos Instrutor (e outras entidades/repositórios) para realizar
    // operações de negócio mais complexas.
}