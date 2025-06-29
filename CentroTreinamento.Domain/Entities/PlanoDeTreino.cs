using System; // Para DateTime

public class PlanoDeTreino 
{
    // Atributos (Propriedades com get privados para controle)
    public int Id { get; private set; } // Identificador único do plano de treino
    public string Objetivo { get; private set; } // Objetivo do plano (e.g., "Ganho de massa", "Emagrecimento")
    public string Exercicios { get; private set; } // Representação dos exercícios (JSON ou string formatada)
    public int InstrutorId { get; private set; } // Chave estrangeira para o Instrutor que criou o plano
    public int AlunoId { get; private set; } // Chave estrangeira para o Aluno a quem o plano pertence
    public DateTime DataCriacao { get; private set; } // Data de criação do plano
    public DateTime DataUltimaAtualizacao { get; private set; } // Data da última atualização do plano
    public string Status { get; private set; } // Estado do plano (e.g., "Ativo", "Inativo", "Concluido")

    // Construtor
    public PlanoDeTreino(int id, string objetivo, string exercicios, int instrutorId, int alunoId)
    {
        // Validações básicas no construtor para garantir um estado inicial válido.
        if (id <= 0)
        {
            throw new ArgumentException("ID do plano de treino deve ser positivo.");
        }
        if (string.IsNullOrWhiteSpace(objetivo))
        {
            throw new ArgumentException("Objetivo do plano de treino não pode ser vazio.");
        }
        if (string.IsNullOrWhiteSpace(exercicios))
        {
            throw new ArgumentException("Exercícios do plano de treino não podem ser vazios.");
        }
        if (instrutorId <= 0)
        {
            throw new ArgumentException("ID do instrutor deve ser positivo.");
        }
        if (alunoId <= 0)
        {
            throw new ArgumentException("ID do aluno deve ser positivo.");
        }

        Id = id;
        Objetivo = objetivo;
        Exercicios = exercicios;
        InstrutorId = instrutorId;
        AlunoId = alunoId;
        DataCriacao = DateTime.UtcNow; // Definido na criação, use UtcNow para padronização
        DataUltimaAtualizacao = DateTime.UtcNow; // Mesma data inicial da criação
        Status = "Ativo"; // Um novo plano geralmente começa como Ativo
    }

    // Métodos Internos da Entidade (Comportamento de domínio)

    /// <summary>
    /// Ativa o plano de treino, tornando-o disponível para uso.
    /// </summary>
    /// <exception cref="InvalidOperationException">Lançada se o plano já estiver ativo ou concluído.</exception>
    public void Ativar()
    {
        if (Status == "Ativo")
        {
            throw new InvalidOperationException("Plano de treino já está ativo.");
        }
        if (Status == "Concluido")
        {
            throw new InvalidOperationException("Plano de treino concluído não pode ser reativado diretamente.");
        }
        Status = "Ativo";
        DataUltimaAtualizacao = DateTime.UtcNow;
    }

    /// <summary>
    /// Desativa o plano de treino, tornando-o indisponível.
    /// </summary>
    /// <exception cref="InvalidOperationException">Lançada se o plano já estiver inativo ou concluído.</exception>
    public void Desativar()
    {
        if (Status == "Inativo")
        {
            throw new InvalidOperationException("Plano de treino já está inativo.");
        }
        if (Status == "Concluido")
        {
            throw new InvalidOperationException("Plano de treino concluído não pode ser desativado (já está em um estado final).");
        }
        Status = "Inativo";
        DataUltimaAtualizacao = DateTime.UtcNow;
    }

    /// <summary>
    /// Marca o plano de treino como concluído. Este é um estado final.
    /// </summary>
    /// <exception cref="InvalidOperationException">Lançada se o plano já estiver concluído.</exception>
    public void MarcarComoConcluido()
    {
        if (Status == "Concluido")
        {
            throw new InvalidOperationException("Plano de treino já está concluído.");
        }
        Status = "Concluido";
        DataUltimaAtualizacao = DateTime.UtcNow;
    }

    /// <summary>
    /// Atualiza os detalhes do objetivo e exercícios do plano de treino.
    /// </summary>
    /// <param name="novoObjetivo">O novo objetivo do plano.</param>
    /// <param name="novosExercicios">A nova string de exercícios do plano.</param>
    /// <exception cref="ArgumentException">Lançada se os novos detalhes forem inválidos.</exception>
    /// <exception cref="InvalidOperationException">Lançada se o plano estiver em um estado que não permite atualização (ex: Concluído).</exception>
    public void AtualizarDetalhes(string novoObjetivo, string novosExercicios)
    {
        if (string.IsNullOrWhiteSpace(novoObjetivo))
        {
            throw new ArgumentException("Novo objetivo não pode ser vazio.");
        }
        if (string.IsNullOrWhiteSpace(novosExercicios))
        {
            throw new ArgumentException("Novos exercícios não podem ser vazios.");
        }
        if (Status == "Concluido") // Exemplo de regra de negócio: um plano concluído não pode ser alterado
        {
            throw new InvalidOperationException("Não é possível atualizar um plano de treino que está concluído.");
        }

        Objetivo = novoObjetivo;
        Exercicios = novosExercicios;
        DataUltimaAtualizacao = DateTime.UtcNow;
    }

    /// <summary>
    /// Retorna uma representação em string do objeto PlanoDeTreino.
    /// </summary>
    /// <returns>Uma string que representa o plano de treino.</returns>
    public override string ToString()
    {
        return $"PlanoDeTreino{{ Id={Id}, Objetivo='{Objetivo}', AlunoId={AlunoId}, InstrutorId={InstrutorId}, Status='{Status}' }}";
    }

    // Os "Serviços" listados (Criação, edição, visualização, associação)
    // NÃO são métodos desta classe de entidade. Eles pertencem a classes de SERVIÇO (ex: PlanoDeTreinoService)
    // que orquestram essas operações, utilizando esta entidade e interagindo com repositórios e outras entidades.
}