// CentroTreinamento.Domain\Enums\StatusAluno.cs
namespace CentroTreinamento.Domain.Enums
{
    public enum StatusAluno
    {
        Ativo = 1,
        Inativo = 2,
        Pendente = 3, // Ex: Aguardando pagamento ou documentação
        Trancado = 4 // Ex: Matrícula trancada temporariamente
    }
}