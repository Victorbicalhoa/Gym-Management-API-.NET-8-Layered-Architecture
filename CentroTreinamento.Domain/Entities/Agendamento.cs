using System; // Para DateTime, TimeSpan

public class Agendamento
{
    // Atributos (Propriedades com get privados para controle)
    public int Id { get; private set; } // Identificador único do agendamento
    public DateTime Data { get; private set; } // Data do agendamento
    public TimeSpan HoraInicio { get; private set; } // Hora de início do agendamento
    public TimeSpan HoraFim { get; private set; } // Hora de fim do agendamento (calculada ou definida)
    public int AlunoId { get; private set; } // Chave estrangeira para o Aluno agendado
    public int InstrutorId { get; private set; } // Chave estrangeira para o Instrutor do agendamento
    public string Status { get; private set; } // Estado do agendamento (e.g., "Pendente", "Confirmado", "Cancelado", "Recusado", "Realizado")
    public DateTime DataCriacao { get; private set; } // Data de criação do agendamento
    public string Observacoes { get; private set; } // Observações adicionais sobre o agendamento (opcional)

    // Construtor
    public Agendamento(int id, DateTime data, TimeSpan horaInicio, TimeSpan horaFim, int alunoId, int instrutorId, string observacoes = null)
    {
        // Validações básicas no construtor para garantir um estado inicial válido.
        if (id <= 0)
        {
            throw new ArgumentException("ID do agendamento deve ser positivo.");
        }
        if (data == default(DateTime)) // Verifica se a data foi fornecida
        {
            throw new ArgumentException("Data do agendamento não pode ser vazia.");
        }
        if (horaInicio == default(TimeSpan) || horaFim == default(TimeSpan))
        {
            throw new ArgumentException("Hora de início e fim do agendamento não podem ser vazias.");
        }
        if (horaInicio >= horaFim)
        {
            throw new ArgumentException("Hora de início deve ser anterior à hora de fim.");
        }
        if (alunoId <= 0)
        {
            throw new ArgumentException("ID do aluno deve ser positivo.");
        }
        if (instrutorId <= 0)
        {
            throw new ArgumentException("ID do instrutor deve ser positivo.");
        }

        Id = id;
        Data = data.Date; // Garante que a data não inclua a parte da hora se não for relevante para o campo 'Data'
        HoraInicio = horaInicio;
        HoraFim = horaFim;
        AlunoId = alunoId;
        InstrutorId = instrutorId;
        Status = "Pendente"; // Um novo agendamento geralmente começa como Pendente
        DataCriacao = DateTime.UtcNow; // Definido na criação, use UtcNow para padronização
        Observacoes = observacoes;
    }

    // Métodos Internos da Entidade (Comportamento de domínio)

    /// <summary>
    /// Confirma o agendamento.
    /// </summary>
    /// <exception cref="InvalidOperationException">Lançada se o agendamento não puder ser confirmado.</exception>
    public void Confirmar()
    {
        if (Status == "Cancelado" || Status == "Recusado" || Status == "Realizado")
        {
            throw new InvalidOperationException($"Não é possível confirmar um agendamento com status '{Status}'.");
        }
        if (Status == "Confirmado")
        {
            // Opcional: throw new InvalidOperationException("Agendamento já está confirmado.");
            return; // Já está no estado desejado
        }
        Status = "Confirmado";
        // Lógica adicional, como registrar um evento ou timestamp de confirmação
    }

    /// <summary>
    /// Recusa o agendamento.
    /// </summary>
    /// <exception cref="InvalidOperationException">Lançada se o agendamento não puder ser recusado.</exception>
    public void Recusar()
    {
        if (Status == "Cancelado" || Status == "Realizado")
        {
            throw new InvalidOperationException($"Não é possível recusar um agendamento com status '{Status}'.");
        }
        if (Status == "Recusado")
        {
            return;
        }
        Status = "Recusado";
        // Lógica adicional, como notificar o aluno
    }

    /// <summary>
    /// Cancela o agendamento.
    /// </summary>
    /// <exception cref="InvalidOperationException">Lançada se o agendamento não puder ser cancelado.</exception>
    public void Cancelar()
    {
        if (Status == "Realizado")
        {
            throw new InvalidOperationException("Não é possível cancelar um agendamento já realizado.");
        }
        if (Status == "Cancelado")
        {
            return;
        }
        Status = "Cancelado";
        // Lógica adicional, como liberar o horário do instrutor ou notificar as partes
    }

    /// <summary>
    /// Marca o agendamento como realizado.
    /// </summary>
    /// <exception cref="InvalidOperationException">Lançada se o agendamento não puder ser marcado como realizado.</exception>
    public void MarcarComoRealizado()
    {
        if (Status == "Cancelado" || Status == "Recusado" || Status == "Realizado")
        {
            throw new InvalidOperationException($"Não é possível marcar como realizado um agendamento com status '{Status}'.");
        }
        if (Status == "Realizado")
        {
            return;
        }
        Status = "Realizado";
        // Lógica adicional, como marcar o check-out se não houver um método separado para isso
    }

    /// <summary>
    /// Registra o check-in do aluno para o agendamento.
    /// </summary>
    /// <exception cref="InvalidOperationException">Lançada se o agendamento não estiver em um estado que permita check-in.</exception>
    public void MarcarCheckin()
    {
        if (Status != "Confirmado") // Só permite check-in se estiver confirmado
        {
            throw new InvalidOperationException($"Não é possível fazer check-in para um agendamento com status '{Status}'. O agendamento deve estar 'Confirmado'.");
        }
        // Aqui, você pode adicionar um atributo 'bool IsCheckedIn' ou criar um estado mais granular como "CheckedIn"
        // Por simplicidade, vou apenas ilustrar a validação.
        // Se quisermos um estado específico: Status = "CheckedIn";
    }

    /// <summary>
    /// Registra o check-out do aluno para o agendamento.
    /// </summary>
    /// <exception cref="InvalidOperationException">Lançada se o agendamento não estiver em um estado que permita check-out.</exception>
    public void MarcarCheckout()
    {
        // Assumindo que check-out só pode ser feito após check-in ou em um estado pós-confirmado.
        // Se houver um estado "CheckedIn", a validação seria if (Status != "CheckedIn")
        if (Status != "Confirmado" && Status != "Realizado") // Permite checkout se Confirmado ou já Realizado
        {
            throw new InvalidOperationException($"Não é possível fazer check-out para um agendamento com status '{Status}'. O agendamento deve estar 'Confirmado' ou 'Realizado'.");
        }
        // Se MarcarComoRealizado for a operação final, este método pode apenas fazer parte dela
        // ou ser um estado intermediário mais detalhado.
        // Por simplicidade, assumindo que MarcarComoRealizado já engloba o fim do ciclo.
    }


    /// <summary>
    /// Retorna uma representação em string do objeto Agendamento.
    /// </summary>
    /// <returns>Uma string que representa o agendamento.</returns>
    public override string ToString()
    {
        return $"Agendamento{{ Id={Id}, Data={Data.ToShortDateString()}, HoraInicio={HoraInicio}, HoraFim={HoraFim}, AlunoId={AlunoId}, InstrutorId={InstrutorId}, Status='{Status}' }}";
    }

    // Os "Serviços" listados (Criação, Gerenciamento do ciclo de vida completo, Consulta)
    // NÃO são métodos desta classe de entidade. Eles pertencem a classes de SERVIÇO (ex: AgendamentoService)
    // que orquestram essas operações, utilizando esta entidade e interagindo com repositórios e outras entidades.
}