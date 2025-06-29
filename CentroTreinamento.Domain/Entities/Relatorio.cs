using System; // Para DateTime

public class Relatorio
{
    // Atributos (Propriedades com get privados para controle)
    public int Id { get; private set; } // Identificador único do relatório
    public DateTime DataGeracao { get; private set; } // Data e hora em que o relatório foi gerado
    public string Conteudo { get; private set; } // O conteúdo do relatório (caminho do arquivo, JSON serializado, etc.)
    public string Tipo { get; private set; } // Tipo do relatório (e.g., "Financeiro Total", "Progresso Aluno")
    public string PeriodoReferencia { get; private set; } // Período de dados que o relatório abrange (e.g., "2023-01", "Q1-2023")
    public int? GeradoPor { get; private set; } // ID opcional do Administrador que gerou o relatório (int? para nullable)
    public string Status { get; private set; } // Estado do relatório (e.g., "Gerado", "Pendente")

    // Construtor
    // Observação: Para relatórios que são gerados assincronamente, o construtor pode iniciar com status "Pendente"
    // e o conteúdo ser associado posteriormente. Se for síncrono, já pode iniciar com "Gerado" e conteúdo.
    public Relatorio(
        int id,
        string tipo,
        string periodoReferencia,
        int? geradoPor = null, // Parâmetro opcional e nullable
        string conteudo = null, // Conteúdo pode ser null inicialmente se for gerado assincronamente
        string statusInicial = "Gerado" // Status padrão ao criar, pode ser "Pendente" para geração assíncrona
    )
    {
        // Validações básicas no construtor.
        if (id <= 0)
        {
            throw new ArgumentException("ID do relatório deve ser positivo.");
        }
        if (string.IsNullOrWhiteSpace(tipo))
        {
            throw new ArgumentException("Tipo do relatório não pode ser vazio.");
        }
        if (string.IsNullOrWhiteSpace(periodoReferencia))
        {
            throw new ArgumentException("Período de referência do relatório não pode ser vazio.");
        }

        Id = id;
        DataGeracao = DateTime.UtcNow; // Data e hora da criação/solicitação do relatório
        Conteudo = conteudo;
        Tipo = tipo;
        PeriodoReferencia = periodoReferencia;
        GeradoPor = geradoPor;

        // Valida e define o status inicial
        if (statusInicial != "Gerado" && statusInicial != "Pendente")
        {
            throw new ArgumentException($"Status inicial '{statusInicial}' inválido para relatório.");
        }
        Status = statusInicial;

        // Se o status inicial é "Gerado" mas o conteúdo é nulo, isso pode indicar um problema
        if (Status == "Gerado" && string.IsNullOrWhiteSpace(Conteudo))
        {
            // Opcional: throw new InvalidOperationException("Relatório com status 'Gerado' deve ter conteúdo.");
            // Ou o serviço que chama o construtor é responsável por garantir isso.
        }
    }

    // Métodos Internos da Entidade (Comportamento de domínio)

    /// <summary>
    /// Marca o relatório como "Gerado".
    /// Utilizado quando um relatório pendente foi concluído.
    /// </summary>
    /// <exception cref="InvalidOperationException">Lançada se o relatório já estiver gerado.</exception>
    public void MarcarComoGerado()
    {
        if (Status == "Gerado")
        {
            throw new InvalidOperationException("Relatório já está com status 'Gerado'.");
        }
        if (string.IsNullOrWhiteSpace(Conteudo))
        {
            // Regra de negócio: um relatório só pode ser marcado como "Gerado" se tiver conteúdo.
            throw new InvalidOperationException("Não é possível marcar o relatório como 'Gerado' sem conteúdo.");
        }
        Status = "Gerado";
        // Opcional: Atualizar DataGeracao para o momento exato em que o conteúdo foi associado/finalizado.
        // DataGeracao = DateTime.UtcNow;
    }

    /// <summary>
    /// Associa o conteúdo (caminho do arquivo ou JSON serializado) ao relatório.
    /// </summary>
    /// <param name="novoConteudo">O caminho para o arquivo do relatório ou o JSON serializado.</param>
    /// <exception cref="ArgumentException">Lançada se o conteúdo for vazio.</exception>
    /// <exception cref="InvalidOperationException">Lançada se o relatório já estiver gerado (e a regra é que o conteúdo não pode mudar depois).</exception>
    public void AssociarConteudo(string novoConteudo)
    {
        if (string.IsNullOrWhiteSpace(novoConteudo))
        {
            throw new ArgumentException("O conteúdo a ser associado não pode ser vazio.");
        }
        // Exemplo de regra: O conteúdo só pode ser associado uma vez ou enquanto o status for "Pendente"
        if (Status == "Gerado" && !string.IsNullOrWhiteSpace(Conteudo))
        {
            throw new InvalidOperationException("O conteúdo deste relatório já foi gerado e não pode ser alterado.");
        }

        Conteudo = novoConteudo;
        DataGeracao = DateTime.UtcNow; // Atualiza a data de geração para o momento em que o conteúdo é finalizado
    }

    // O método de "marcar como pendente" pode ser implícito ao criar o relatório com o status inicial "Pendente"
    // Ou ter um método explícito se houver transições de estado para "Pendente" a partir de outros estados.
    /*
    public void MarcarComoPendente()
    {
        if (Status == "Gerado")
        {
            throw new InvalidOperationException("Não é possível marcar um relatório 'Gerado' como 'Pendente'.");
        }
        Status = "Pendente";
    }
    */

    /// <summary>
    /// Retorna uma representação em string do objeto Relatorio.
    /// </summary>
    /// <returns>Uma string que representa o relatório.</returns>
    public override string ToString()
    {
        return $"Relatorio{{ Id={Id}, Tipo='{Tipo}', Periodo='{PeriodoReferencia}', Status='{Status}', DataGeracao={DataGeracao.ToShortDateString()} }}";
    }

    // Os "Serviços" listados (Geração, Exportação, Armazenamento, Consulta)
    // NÃO são métodos desta classe de entidade. Eles pertencem a classes de SERVIÇO (ex: RelatorioService)
    // que orquestram essas operações complexas, utilizando esta entidade e interagindo
    // com repositórios, sistemas de arquivos/armazenamento e outras entidades.
}