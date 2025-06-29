using System; // Para DateTime

public class Pagamento
{
    // Atributos (Propriedades com get privados para controle)
    public int Id { get; private set; } // Identificador único do pagamento
    public double Valor { get; private set; } // Valor do pagamento
    public DateTime DataPagamento { get; private set; } // Data em que o pagamento foi efetivado/registrado
    public DateTime DataVencimento { get; private set; } // Data de vencimento do pagamento
    public string Status { get; private set; } // Estado atual do pagamento (e.g., "Pago", "Pendente", "Vencido", "Estornado")
    public int AlunoId { get; private set; } // Chave estrangeira para o Aluno que realizou o pagamento
    public string TipoServico { get; private set; } // Tipo de serviço pago (e.g., "Mensalidade", "Treino Avulso")
    public string MetodoPagamento { get; private set; } // Método de pagamento (e.g., "Cartao de Credito", "Dinheiro", "Pix")
    public string ReferenciaTransacaoExterna { get; private set; } // ID da transação em um gateway de pagamento (opcional)

    // Construtor
    public Pagamento(
        int id,
        double valor,
        DateTime dataPagamento,
        DateTime dataVencimento,
        int alunoId,
        string tipoServico,
        string metodoPagamento,
        string referenciaTransacaoExterna = null) // referenciaTransacaoExterna é opcional
    {
        // Validações básicas no construtor para garantir um estado inicial válido.
        if (id <= 0)
        {
            throw new ArgumentException("ID do pagamento deve ser positivo.");
        }
        if (valor <= 0)
        {
            throw new ArgumentException("Valor do pagamento deve ser positivo.");
        }
        if (dataPagamento == default(DateTime))
        {
            throw new ArgumentException("Data de pagamento não pode ser vazia.");
        }
        if (dataVencimento == default(DateTime))
        {
            throw new ArgumentException("Data de vencimento não pode ser vazia.");
        }
        if (alunoId <= 0)
        {
            throw new ArgumentException("ID do aluno deve ser positivo.");
        }
        if (string.IsNullOrWhiteSpace(tipoServico))
        {
            throw new ArgumentException("Tipo de serviço não pode ser vazio.");
        }
        if (string.IsNullOrWhiteSpace(metodoPagamento))
        {
            throw new ArgumentException("Método de pagamento não pode ser vazio.");
        }

        Id = id;
        Valor = valor;
        DataPagamento = dataPagamento.Date; // Considera apenas a data, se a hora não for essencial para este atributo
        DataVencimento = dataVencimento.Date; // Considera apenas a data
        AlunoId = alunoId;
        TipoServico = tipoServico;
        MetodoPagamento = metodoPagamento;
        ReferenciaTransacaoExterna = referenciaTransacaoExterna;
        Status = "Pendente"; // Um novo pagamento geralmente começa como Pendente, até ser confirmado.
        // Ou poderia ser "Pago" se for um registro manual pós-pagamento. Depende do fluxo de negócio principal.
        // Para este exemplo, vou considerar "Pendente" como default para pagamentos a serem confirmados.
        // Se for um pagamento já "efetuado", o serviço pode chamar 'MarcarComoPago()' logo após a criação.
    }

    // Métodos Internos da Entidade (Comportamento de domínio)

    /// <summary>
    /// Marca o pagamento como "Pago".
    /// </summary>
    /// <exception cref="InvalidOperationException">Lançada se o pagamento já estiver em um estado final ou inválido para ser pago.</exception>
    public void MarcarComoPago()
    {
        if (Status == "Pago" || Status == "Estornado")
        {
            throw new InvalidOperationException($"Não é possível marcar como pago um pagamento com status '{Status}'.");
        }
        Status = "Pago";
        // Lógica adicional, como registrar a data/hora exata da marcação como pago
    }

    /// <summary>
    /// Marca o pagamento como "Vencido".
    /// </summary>
    /// <exception cref="InvalidOperationException">Lançada se o pagamento já estiver em um estado final.</exception>
    public void MarcarComoVencido()
    {
        if (Status == "Pago" || Status == "Estornado")
        {
            throw new InvalidOperationException($"Não é possível marcar como vencido um pagamento com status '{Status}'.");
        }
        if (Status == "Vencido")
        {
            return; // Já está no estado desejado
        }
        Status = "Vencido";
    }

    /// <summary>
    /// Marca o pagamento como "Pendente".
    /// Geralmente usado para reverter um estado ou definir um pagamento que aguarda confirmação.
    /// </summary>
    /// <exception cref="InvalidOperationException">Lançada se o pagamento já estiver em um estado final.</exception>
    public void MarcarComoPendente()
    {
        if (Status == "Estornado" || Status == "Pago")
        {
            throw new InvalidOperationException($"Não é possível marcar como pendente um pagamento com status '{Status}'.");
        }
        if (Status == "Pendente")
        {
            return; // Já está no estado desejado
        }
        Status = "Pendente";
    }

    /// <summary>
    /// Estorna o pagamento, marcando-o como "Estornado". Este é um estado final.
    /// </summary>
    /// <exception cref="InvalidOperationException">Lançada se o pagamento já estiver estornado ou não for pago.</exception>
    public void Estornar()
    {
        if (Status == "Estornado")
        {
            throw new InvalidOperationException("Pagamento já está estornado.");
        }
        // Uma regra de negócio comum é só poder estornar pagamentos que foram previamente pagos.
        if (Status != "Pago")
        {
            throw new InvalidOperationException("Somente pagamentos com status 'Pago' podem ser estornados.");
        }
        Status = "Estornado";
        // Lógica adicional, como notificar sistemas externos ou o aluno
    }

    /// <summary>
    /// Atualiza dados específicos do pagamento.
    /// NOTA: Cuidado ao expor este método. Geralmente, Pagamentos são imutáveis após a criação para auditoria.
    /// Alterações deveriam ser estornos, novas transações, etc. Este método é para ajustes muito específicos e controlados.
    /// </summary>
    /// <param name="novoValor">O novo valor do pagamento.</param>
    /// <param name="novaDataPagamento">A nova data de efetivação do pagamento.</param>
    /// <param name="novoStatus">O novo status (use os métodos específicos de status se possível).</param>
    /// <exception cref="InvalidOperationException">Lançada se o pagamento não puder ser atualizado.</exception>
    /// <exception cref="ArgumentException">Lançada se os novos dados forem inválidos.</exception>
    public void AtualizarDadosPagamento(double novoValor, DateTime novaDataPagamento, string novoStatus)
    {
        // Regra de negócio: Pagamentos estornados ou já pagos (e não estornados) não devem ser modificados diretamente.
        if (Status == "Estornado" || Status == "Pago")
        {
            throw new InvalidOperationException($"Não é possível atualizar dados de um pagamento com status '{Status}'. Use métodos específicos como Estornar.");
        }

        if (novoValor <= 0)
        {
            throw new ArgumentException("Novo valor do pagamento deve ser positivo.");
        }
        if (novaDataPagamento == default(DateTime))
        {
            throw new ArgumentException("Nova data de pagamento não pode ser vazia.");
        }
        if (string.IsNullOrWhiteSpace(novoStatus))
        {
            throw new ArgumentException("Novo status não pode ser vazio.");
        }
        // Validar novoStatus contra os estados permitidos (Pago, Pendente, Vencido, Estornado)
        if (novoStatus != "Pago" && novoStatus != "Pendente" && novoStatus != "Vencido" && novoStatus != "Estornado")
        {
            throw new ArgumentException($"Novo status '{novoStatus}' inválido.");
        }

        Valor = novoValor;
        DataPagamento = novaDataPagamento.Date;
        Status = novoStatus; // O ideal é usar os métodos de status específicos (MarcarComoPago, etc.)
                             // Este setter direto de status é perigoso se não houver validação forte em quem o chama.
                             // Pode ser removido e forçar o uso dos métodos de transição de status.
    }


    /// <summary>
    /// Retorna uma representação em string do objeto Pagamento.
    /// </summary>
    /// <returns>Uma string que representa o pagamento.</returns>
    public override string ToString()
    {
        return $"Pagamento{{ Id={Id}, Valor={Valor:C}, DataPagamento={DataPagamento.ToShortDateString()}, AlunoId={AlunoId}, Status='{Status}' }}";
    }

    // Os "Serviços" listados (Registro, Processamento, Notificação, Auditoria)
    // NÃO são métodos desta classe de entidade. Eles pertencem a classes de SERVIÇO (ex: PagamentoService,
    // NotificacaoService) que orquestram essas operações complexas, utilizando esta entidade e interagindo
    // com repositórios, gateways de pagamento e outras entidades.
}